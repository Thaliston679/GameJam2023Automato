using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Automatos", order = 1)]
public class Automatos : ScriptableObject
{
    public string automatoName;//Nome do automato

    public Sprite icon;

    public int[] id;
    public int[] idM;
    public int[] orderIdM;
}