using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStats", menuName = "Scriptable Objects/ItemStats")]
public class ItemStats : ScriptableObject
{
    public Sprite[] sprites;
    public ItemType itemType;
    public string description;
    
    [Header("Size")]
    public int height;
    public int width;
    
    [Header("Shape")]
    public List<bool> dimensions;
}

public enum ItemType
{
    None = 0,
    Nieprzeciętnie_wielki_baton_proteinowy = 1,
    Lanyard = 2,
    Niszczyciel_dobrej_zabawy = 3,
    Starożytny_złoty_bożek = 4,
    Złamana_NDA = 5
}
