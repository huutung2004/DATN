using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class BaseWeakpon : BaseObj
{
    [SerializeField] public Transform m_mesh;
    public virtual void IdleWeapon()
    {
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
        IdleWeapon();
    }

}
