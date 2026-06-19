using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : BaseObj, ITakeDamage
{
    [SerializeField] public float maxHp = 100;
    public float currentHp;
    private Animator animator;
    [SerializeField] private Sprite sprite;
    public NavMeshAgent Agent;

    private void OnValidate()
    {
        animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
    }
    private void Awake()
    {
        if(animator == null) animator = GetComponent<Animator>();
        currentHp = maxHp;
    }
    private void OnEnable()
    {
        Tween.Delay(0.4f, () =>
        {
            ParticalManager.Instance.PlaySomke(transform.position + Vector3.up * 0.5f);
        });
    }
    public void TakeDamage(float damge)
    {
        animator.SetTrigger("damage");
        if (ParticalManager.Instance)
        {
            ParticalManager.Instance.PlayEffect(ParticalManager.Instance.m_hurtEffect, transform.position + Vector3.up * 0.5f);
        }
        currentHp = (int)Mathf.Clamp(currentHp - damge, 0f, maxHp);
        // UpdateUI();
        HealBarPopup.Instance.GetImage().fillAmount = currentHp / maxHp;
        if (currentHp <= 0)
        {
            Debug.Log("Player die");
            ParticalManager.Instance.PlayEffect(ParticalManager.Instance.m_deathEffect, transform.position + Vector3.up * 0.5f);
            if (EnemySpawner.Instance != null)
                EnemySpawner.Instance.ReturnEnemy(this);
            else
                gameObject.SetActive(false);

        }
    }
    public override void Holding()
    {
        if (m_outline)
            m_outline.enabled = true;
        m_canInteract = true;
        if (HealBarPopup.Instance)
        {
            HealBarPopup.Instance.FillData(m_nameOfObj, sprite, currentHp / maxHp);
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
