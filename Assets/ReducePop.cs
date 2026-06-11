using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReducePop : BasePopup
{
    public static ReducePop Instance;
    [SerializeField] private TMP_Text m_infor;
    [SerializeField] private Image m_sprite;

    [Header("Animation")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float startX = -500f;
    [SerializeField] private float endX = 50f;
    [SerializeField] private float moveDuration = 0.4f;
    [SerializeField] private float stayTime = 2f;

    private Coroutine hideCoroutine;
    private void Start()
    {
        Instance = this;
    }
    public void FillData(string title, Sprite sprite)
    {
        m_infor.SetText(title);
        m_sprite.sprite = sprite;

        ShowLootPopup();
    }

    private void ShowLootPopup()
    {
        // Dừng tween cũ
        Tween.StopAll(rectTransform);
        Tween.StopAll(main);

        // Hủy auto hide cũ
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // Hiện popup
        gameObject.SetActive(true);
        isShow = true;
        main.alpha = 1f;

        // Reset vị trí về bên trái
        Vector2 pos = rectTransform.anchoredPosition;
        pos.x = startX;
        rectTransform.anchoredPosition = pos;

        // Kéo từ trái sang phải
        Tween.UIAnchoredPositionX(
            rectTransform,
            endX,
            moveDuration,
            Ease.OutBack
        );

        // Tự ẩn sau vài giây
        hideCoroutine = StartCoroutine(AutoHide());
    }

    private IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(stayTime);

        Hide();
    }

    public override void Hide()
    {
        Tween.StopAll(rectTransform);

        Tween.Alpha(main, 0f, 0.25f)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        isShow = false;
    }
}
