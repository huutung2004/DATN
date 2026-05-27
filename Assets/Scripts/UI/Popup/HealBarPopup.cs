using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealBarPopup : BasePopup
{
    [SerializeField] private TMP_Text m_nameOfObj;
    [SerializeField] private Image m_iconRequirment;
    [SerializeField] private Vector2 m_offset = new Vector2(-40, -40);
    [SerializeField] private RectTransform m_mainRect;
    [SerializeField] private Image m_imageFill;

    public static HealBarPopup Instance;
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    private void Start()
    {
        Hide();
    }
    private void OnEnable()
    {
        
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        
    }
    public void FillData(string name, Sprite icon, float percentHp)
    {
        m_nameOfObj.SetText(name);
        m_iconRequirment.sprite = icon;
        m_imageFill.fillAmount = percentHp;
    }
    public void UpdateHpBar()
    {
        
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
