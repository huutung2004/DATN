using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestable : BaseObj
{
    protected bool m_canHarvest = false;
    public override string GetPromt() =>"PickUp";

    public override void Interact()
    {
        base.Interact();
    }
    public override void UnHolding()
    {
        base.UnHolding();
    }
}

