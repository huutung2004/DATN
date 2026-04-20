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
    [SerializeField] private Slot m_slot;

    private bool m_isSelected = false;
    private static event Action<HotBar> requestTurnOf;

    private void Awake()
    {
        if (m_imgRender == null || m_unSelectSr == null || m_selectedSr == null)
        {
            Debug.LogWarning("Missing Ref");
            return;
        }

        if (m_hotBarBtn != null)
            m_hotBarBtn.onClick.AddListener(ToggleBar);
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
    public void ToggleBar()
    {
        if (m_isSelected)
            Deselect();
        else
            Select();
    }
    private void TurnOff(HotBar hotbar)
    {
        if (hotbar == this) return;

        Deselect();
    }
    public void Select()
    {
        if (m_isSelected) return;

        m_isSelected = true;
        m_imgRender.sprite = m_selectedSr;

        Tween.UIAnchoredPosition(m_imgRect, new Vector2(0, m_distance), 0.1f, Ease.OutCirc);
        requestTurnOf?.Invoke(this);
    }

    public void Deselect()
    {
        if (!m_isSelected) return;

        m_isSelected = false;
        m_imgRender.sprite = m_unSelectSr;

        Tween.UIAnchoredPosition(m_imgRect, Vector2.zero, 0.1f, Ease.InCirc);
    }
}
