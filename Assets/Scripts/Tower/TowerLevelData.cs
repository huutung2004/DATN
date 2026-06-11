using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerLevelData
{
    public float heal;
    public int level;

    public float damage;

    public float range;

    public float attackSpeed;

    public int woodCost;

    public int stoneCost;
    public int reWood;
    public int reStone;

    public Mesh mesh_Stand;
    public Mesh mesh_Gun;
}
