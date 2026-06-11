using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerData : ScriptableObject
{
    public List<TowerEntry> towerEntries;
}
[System.Serializable]
public struct TowerEntry
{
    public string name;
    public TowerType towerType;
    public TowerLevelData[] levels;
}