using UnityEngine;

[CreateAssetMenu(fileName = "ItemStats", menuName = "Scriptable Objects/ItemStats")]
public class ItemStats : ScriptableObject
{
    public Sprite[] sprites;
    public ItemType itemType;
    public int[] dimensions;
    public string description;
}

public enum ItemType
{
    Crown = 0
}
