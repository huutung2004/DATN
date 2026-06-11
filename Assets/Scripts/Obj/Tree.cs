using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : BaseObj, ITakeDamage
{
    public Sprite m_iconRequired;
    public float m_maxHp = 5;
    public float m_currentHp;
    [Header("Item droped")]
    public int woodDrop = 3;
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
            HealBarPopup.Instance.FillData(m_nameOfObj, m_iconRequired, m_currentHp / m_maxHp);
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

    public void TakeDamage(float damage)
    {
        m_currentHp = Mathf.Clamp(m_currentHp - damage, 0, m_maxHp);

        if (HealBarPopup.Instance)
        {
            HealBarPopup.Instance.FillData(
                m_nameOfObj,
                m_iconRequired,
                m_currentHp / m_maxHp
            );
        }

        if (m_currentHp <= 0)
        {
            InventoryManager.Instance.AddItemByType(ItemType.wood,woodDrop);
            Destroy(gameObject);
            Debug.Log("Thay Pool");
        }
    }
}
