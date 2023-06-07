using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerT : MonoBehaviour
{
    public float speed = 5f; // Velocidade de movimento
    public float speedTarget;
    public GameObject[] esteira;
    public GameObject[] spawnPartsPoints;
    public List<Sprite> partsSpr;
    public List<GameObject> parts;
    public float tempo;
    public GameObject partPref;
    public List<bool> montagem;
    public List<SpriteRenderer> montagemSprR;
    public GameObject montagemSprGO;
    public Transform montagemOrigin;
    public List<Sprite> montagemSpr;
    public int activeID = 999999999;
    public Automatos[] automatosScrObj;
    public GameObject partOnTreadmill = null;
    public int activeLevel = 0;
    public bool activeGame = false;
    private float movement;
    public Image iconImg;
    public List<GameObject> collectingObj = null;
    public Transform targetCollect;
    public Animator animControl;
    public int hp = 3;
    public AudioSource audioSource;
    public AudioClip damageAC;
    public AudioClip collectAC;
    public Animator veyAnim;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public Image scoreimg;
    public Image hpImg;
    public Image victoryImg;
    public Animator victoryAnim;


    void Start()
    {
        /*PlayerPrefs.SetInt("Fase", 0);*/

        StartConfig();
    }

    public void StartConfig()
    {
        //Level padrão ou carrega level
        if (PlayerPrefs.HasKey("Fase"))
        {
            if (PlayerPrefs.GetInt("Fase") < automatosScrObj.Length)
            {
                activeLevel = PlayerPrefs.GetInt("Fase");
            }
            else
            {
                PlayerPrefs.SetInt("Fase", 0);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Fase", 0);
        }

        //Passa controle de coletaveis pra cada level
        for (int i = 0; i < automatosScrObj[activeLevel].id.Length; i++)
        {
            montagem.Add(false);
        }

        //Cria as peças do modelo de acordo com qtd de cada automato
        for(int i = 0; i < automatosScrObj[activeLevel].idM.Length; i++)
        {
            GameObject mGO = Instantiate(montagemSprGO, montagemOrigin);
            SpriteRenderer mSpr = mGO.GetComponent<SpriteRenderer>();
            montagemSprR.Add(mSpr);
            mSpr.color = new(0,0,0,0);
            mSpr.sprite = montagemSpr[automatosScrObj[activeLevel].idM[i]];
            mSpr.sortingOrder = automatosScrObj[activeLevel].orderIdM[i]+2;

            if(i == automatosScrObj[activeLevel].idM.Length - 1)
            {
                mSpr.color = new(1, 1, 1, 1);
            }
        }

        //Passa icone de silhueta
        iconImg.sprite = automatosScrObj[activeLevel].icon;
    }

    private void LateUpdate()
    {
        hpImg.rectTransform.sizeDelta = new(128 * hp, 128);
    }

    public void PlayGame()
    {
        activeGame = true;
    }


    void Update()
    {
        if (activeGame)
        {
            tempo += Time.deltaTime;
            movement = speed * Time.deltaTime;

            if (speed < speedTarget) speed += Time.deltaTime / 10;

            MoveTreadmill();

            CheckDestroy();

            ClickControl();

            CollectMove();
        }
        
    }

    public void CheckDestroy()
    {
        //Destroi partes na fornalha
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].transform.position.x >= 12)
            {
                GameObject destuir = parts[i];
                parts.Remove(destuir);
                Destroy(destuir);
            }
        }

        if (parts.Count > 0)
        {
            for (int i = 0; i > parts.Count; i++)
            {
                if (parts[i].transform.position.x > 10)
                {
                    parts.RemoveAt(i);
                }
            }
        }
    }

    public void ClickControl()
    {
        //Clique na tela
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ReceiveClick();
        }
        else if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            ReceiveClick();
        }
    }

    public void ReceiveClick()
    {
        GameObject coletar = partOnTreadmill;

        if (coletar != null)
        {
            veyAnim.SetTrigger("Atk");
            parts.Remove(coletar);
            collectingObj.Add(coletar);
            activeID = 999999999;
            partOnTreadmill = null;
        }
    }

    public void InCollectPart(Sprite spr, GameObject partIn)
    {
        if (!partIn.GetComponent<PartItem>().checkItem)
        {
            partIn.GetComponent<PartItem>().checkItem = true;
            int idAtivo = 999999999;

            if (partsSpr.Contains(spr))
            {
                idAtivo = partsSpr.IndexOf(spr);
            }


            GameObject destuir = partIn;

            if (destuir != null)
            {
                bool trueDamage = true;

                for (int i = 0; i < automatosScrObj[activeLevel].id.Length; i++)
                {
                    if (partsSpr[idAtivo] == partsSpr[automatosScrObj[activeLevel].id[i]])
                    {
                        //Verifica peça coletada ou não
                        if (montagem[i])
                        {
                            CheckCollectState(0);
                        }
                        else
                        {
                            trueDamage = false;
                            PlaySound(collectAC);
                            montagem[i] = true;
                            if (CheckFinish())
                            {
                                activeGame = false;
                                Invoke(nameof(ScreenFinish), 0.25f);
                            }
                        }
                    }
                    else
                    {
                        CheckCollectState(1);
                    }
                }

                if (trueDamage) CheckDamage();

                collectingObj.Remove(destuir);

                Destroy(destuir);
            }
        }
    }

    public void ScreenFinish()
    {
        victoryPanel.SetActive(true);
        if (activeLevel < automatosScrObj.Length) PlayerPrefs.SetInt("Fase", PlayerPrefs.GetInt("Fase") + 1);
        victoryImg.sprite = automatosScrObj[activeLevel].icon;
        scoreimg.rectTransform.sizeDelta = new(128 * hp, 128);


        if(victoryAnim != null)
        {
            victoryAnim.SetInteger("ID", activeLevel);
            victoryAnim.SetTrigger("Anim");
        }
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void ScreenGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void CheckDamage()
    {
        animControl.SetTrigger("Damage"); 
        speed += 4; 
        hp--;
        PlaySound(damageAC);

        if(hp <= 0)
        {
            Invoke(nameof(ScreenGameOver), 0.25f);
        }
    }

    public void CheckCollectState(int stateCol)
    {
        if(stateCol == 0)
        {
            Debug.Log("Peça já coletada!");
        }

        if (stateCol == 1)
        {
            Debug.Log("Peça errada!");
        }
    }

    public void MoveTreadmill()
    {
        // Move esteira
        for (int i = 0; i < esteira.Length; i++)
        {
            if (esteira[i].transform.position.x > 30)
            {
                Vector3 posEsteira = esteira[i].transform.position;
                posEsteira.x = -30;
                esteira[i].transform.position = posEsteira;
            }

            esteira[i].transform.Translate(Vector2.right * movement);
        }

        // Move pontos de spawn de partes
        for (int i = 0; i < spawnPartsPoints.Length; i++)
        {
            if (spawnPartsPoints[i].transform.position.x > 12)
            {
                Vector3 posSpawnP = spawnPartsPoints[i].transform.position;
                posSpawnP.x = -12;
                spawnPartsPoints[i].transform.position = posSpawnP;

                //Randomiza a criação de partes
                if (i % 2 == 0)
                {
                    GameObject partInst = Instantiate(partPref, spawnPartsPoints[i].transform.position, Quaternion.identity);
                    partInst.transform.SetParent(spawnPartsPoints[i].transform);
                    partInst.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-15, 15));
                    partInst.GetComponent<SpriteRenderer>().sprite = partsSpr[Random.Range(0, partsSpr.Count)];
                    parts.Add(partInst);
                    /*int rand = 0;
                    rand = Random.Range(0, 5);
                    if (rand > 1)
                    {
                        //
                    }*/
                }
                //
            }

            spawnPartsPoints[i].transform.Translate(Vector2.right * movement);
        }
    }

    public bool CheckFinish()
    {
        bool finish = true;
        for(int i = 0; i < montagem.Count; i++)
        {
            if (!montagem[i]) finish = false;

            if(montagem[i] && montagemSprR[i].color.a == 0)
            {
                montagemSprR[i].color = new(1, 1, 1, 1);
            }
        }
        return finish;
    }

    public void PartInArea(Sprite spr, GameObject partIn)
    {
        if (partsSpr.Contains(spr))
        {
            activeID = partsSpr.IndexOf(spr);
        }

        partOnTreadmill = partIn;

    }
    public void PartOutArea(GameObject partOut)
    {
        if(partOnTreadmill == partOut)
        {
            partOnTreadmill = null;
        }
        activeID = 999999999;
    }

    public void CollectMove()
    {
        if(collectingObj != null && collectingObj.Count > 0)
        {
            for(int i = 0; i < collectingObj.Count; i++)
            {
                collectingObj[i].transform.position = Vector3.MoveTowards(collectingObj[i].transform.position, targetCollect.position, (speed * 3) * Time.deltaTime);
            }
            
        }
    }

    public void PlaySound(AudioClip playC)
    {
        audioSource.PlayOneShot(playC);
    }
}
