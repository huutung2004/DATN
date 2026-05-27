using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class BaseWeakpon : BaseObj
{
    [SerializeField] public Transform m_mesh;
    public override string GetPromt() => "PickUp";
    protected override void IdleObj()
    {
        base.IdleObj();
        if (!m_inInventory)
            Tween.LocalPositionY(
                m_mesh,
                m_mesh.localPosition.y + 0.003f,
                0.5f,
                Ease.InOutSine,
                cycles: -1,
                cycleMode: CycleMode.Yoyo
            );
        
    }
    protected override void Start()
    {
        base.Start();
    }
    

}
