using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoldingPopup : BasePopup
{
    [SerializeField] private TMP_Text m_nameOfObj;
    [SerializeField] private Image m_icon;
    [SerializeField] private TMP_Text m_actionText;
    [SerializeField] private Vector2 m_offset = new Vector2(20, 20);
    [SerializeField] private RectTransform m_mainRect;
    [SerializeField] private TMP_Text m_default;

    public static HoldingPopup Instance;
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    private void Start()
    {
        Hide();
    }
    public void FillData(string name, Sprite icon, string action)
    {
        m_nameOfObj.SetText(name);
        if (icon != null)
        {
            m_icon.sprite = icon;
            m_default.enabled = true;
        }
        else
        {
            m_default.enabled = true;
        }
        m_actionText.SetText(action);
    }
    public override void Hide()
    {
        main.gameObject.SetActive(false);
    }
    public override void Show()
    {
        {
            main.gameObject.SetActive(true);

            Canvas canvas = GetComponentInParent<Canvas>();
            RectTransform rectTransform = m_mainRect;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out localPoint
            );

            rectTransform.anchoredPosition = localPoint + m_offset;
        }
    }

}
