using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHeal : MonoBehaviour, ITakeDamage
{
    private bool hasLose = false;
    public TMP_Text m_textheal;
    public int maxheal = 200;
    public int currentheal;
    public Animator animator;
    public void OnValidate()
    {
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(float damge)
    {
        if (damge > 0)
            animator.SetTrigger("damage");
        if (ParticalManager.Instance)
        {
            ParticalManager.Instance.PlayEffect(ParticalManager.Instance.m_hurtEffect, transform.position + Vector3.up * 0.5f);
        }
        currentheal = (int)Mathf.Clamp(currentheal - damge, 0f, maxheal);
        UpdateUI();
        if (currentheal <= 0)
        {
            if (!hasLose)
            {
            hasLose = true;
            LosePopup.Instance.Show();
            }
            // Time.timeScale = 0;
            Debug.Log("Player die");
            // Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentheal = maxheal;
        UpdateUI();
    }
    public void UpdateUI()
    {
        m_textheal.SetText($"{currentheal}");
    }
}
