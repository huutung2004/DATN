using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : BaseObj, ITakeDamage
{
    [SerializeField] private int woodCost = 5;
    [SerializeField] private bool isHome = false;
    public int WoodCost => woodCost;
    private float maxHP = 50f;
    [SerializeField] private float hp = 50;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Sprite home;
    [SerializeField] private float maxHPHome = 500f;
    private void Awake()
    {
        if (isHome) hp = maxHPHome;
        else
            hp = maxHP;
    }
    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (!isHome)
            HealBarPopup.Instance.GetImage().fillAmount = hp / maxHP;
        else
            HealBarPopup.Instance.GetImage().fillAmount = hp / maxHPHome;
        if (hp <= 0)
        {
            if (ParticalManager.Instance)
                if (ParticalManager.Instance.m_smoke)
                    ParticalManager.Instance.PlaySomke(gameObject.transform.position + Vector3.up * 0.6f);
            if (isHome)
            {
                LosePopup.Instance.Show();
                Debug.Log("Lose game");
            }
            Destroy(gameObject);
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
    public override void Holding()
    {
        if (m_outline)
            m_outline.enabled = true;
        m_canInteract = true;
        if (HealBarPopup.Instance)
        {
            HealBarPopup.Instance.FillData(m_nameOfObj, sprite, hp / maxHP);
            HealBarPopup.Instance.Show();
        }

    }


}
