using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : BaseObj
{
    [SerializeField] private int woodCost = 5;

    public int WoodCost => woodCost;

}
