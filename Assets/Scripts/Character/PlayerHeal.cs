using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHeal : MonoBehaviour, ITakeDamage
{
    public TMP_Text m_textheal;
    public int maxheal = 200;
    public int currentheal;

    public void TakeDamage(float damge)
    {
        currentheal = (int)Mathf.Clamp(currentheal - damge, 0f, maxheal);
        UpdateUI();

        if (currentheal <= 0)
        {
            Debug.Log("Player die");
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
