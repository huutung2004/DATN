using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class HotBar : MonoBehaviour
{
    [Header("For UI")]
    [SerializeField] private Button m_hotBarBtn;
    [SerializeField] private Image m_imgRender;
    [SerializeField] private Sprite m_unSelectSr;
    [SerializeField] private Sprite m_selectedSr;

    [Header("Effect")]
    [SerializeField] private RectTransform m_imgRect;
    [SerializeField] private float m_distance;

    private bool m_isSelected = false;
    private static event Action<HotBar> requestTurnOf;

    private void Awake()
    {
        if (m_imgRender == null || m_imgRender == null || m_unSelectSr == null || m_selectedSr == null)
        {
            Debug.LogWarning("Missing Ref");
            return;
        }
        if (m_hotBarBtn != null) m_hotBarBtn.onClick.AddListener(ToggleBar);
        m_imgRender.sprite = m_unSelectSr;
    }
    private void OnDisable()
    {
        requestTurnOf -= TurnOff;

    }
    private void OnEnable()
    {
        requestTurnOf += TurnOff;
    }
    private void ToggleBar()
    {
        m_isSelected = !m_isSelected;
        if (m_isSelected)
        {
            m_imgRender.sprite = m_selectedSr;
            Tween.UIAnchoredPosition(m_imgRect, new Vector2(0, m_distance), 0.1f, Ease.OutCirc);
            requestTurnOf?.Invoke(this);
        }
        else
        {
            m_imgRender.sprite = m_unSelectSr;
            if (m_imgRect.anchoredPosition != Vector2.zero)
            {
                Tween.UIAnchoredPosition(m_imgRect, Vector2.zero, 0.1f, Ease.InCirc);
            }
        }
    }
    private void TurnOff(HotBar hotbar)
    {
        if (hotbar == this) return;
        m_isSelected = false;
        m_imgRender.sprite = m_unSelectSr;
        if (m_imgRect.anchoredPosition != Vector2.zero)
        {
            Tween.UIAnchoredPosition(m_imgRect, Vector2.zero, 0.1f, Ease.InBack);
        }
    }
}
