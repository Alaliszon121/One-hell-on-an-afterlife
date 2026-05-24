using System.Collections.Generic;
using UnityEngine;

public class SuitcaseScript : MonoBehaviour
{
    public List<GameObject> itemsInSuitcase = new List<GameObject>();
    public Dictionary<Vector2Int, ItemType> spaceInSuitcase =  new Dictionary<Vector2Int, ItemType>();
}
