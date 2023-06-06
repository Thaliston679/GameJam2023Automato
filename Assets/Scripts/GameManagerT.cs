using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool[] montagem;
    public int activeID = 999999999;
    public Automatos[] automatosScrObj;

    void Start()
    {
        
    }


    void Update()
    {
        tempo += Time.deltaTime;
        float movement = speed * Time.deltaTime;

        if (speed < speedTarget) speed += Time.deltaTime / 10;

        // Move esteira
        for(int i = 0; i < esteira.Length; i++)
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
                if(i % 2 == 0)
                {
                    GameObject partInst = Instantiate(partPref, spawnPartsPoints[i].transform.position, Quaternion.identity);
                    partInst.transform.SetParent(spawnPartsPoints[i].transform);
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

        //Destroi partes na fornalha
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].transform.position.x >= 12)
            {
                GameObject destuir = parts[i];
                parts.Remove(destuir);
                Debug.Log("-1");
                Destroy(destuir);
            }
        }

        // Move partes
        if (parts.Count > 0)
        {
            for(int i = 0; i > parts.Count; i++)
            {
                if (parts[i].transform.position.x > 10)
                {
                    parts.RemoveAt(i);
                    Debug.Log(tempo);
                }
            }
        }
    }

    public void PartInArea(Sprite spr)
    {
        if (partsSpr.Contains(spr))
        {
            activeID = partsSpr.IndexOf(spr);
        }
        Debug.Log("Entrou");
    }
    public void PartOutArea()
    {
        Debug.Log("Saiu");
        activeID = 999999999;
    }
}
