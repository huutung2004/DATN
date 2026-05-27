using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : BaseObj
{
    public Sprite m_iconRequired;
    public float m_maxHp = 5;
    public float m_currentHp;
    protected override void Start()
    {
        base.Start();
        m_currentHp = m_maxHp;
    }
    public override void Holding()
    {
        base.Holding();
        if (HealBarPopup.Instance)
        {
            HealBarPopup.Instance.FillData(m_nameOfObj,m_iconRequired, m_currentHp/m_maxHp);
            HealBarPopup.Instance.Show();
        }
    }
    public override void UnHolding()
    {
        base.UnHolding();
        if (HealBarPopup.Instance)
        {
            HealBarPopup.Instance.Hide();
        }
    }


}
