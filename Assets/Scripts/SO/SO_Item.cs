using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="ItemData", menuName ="Data/Items")]
public class SO_Item : ScriptableObject
{
    public List<ItemData> items;
}
