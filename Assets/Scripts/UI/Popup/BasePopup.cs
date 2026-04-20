using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class BasePopup : MonoBehaviour
{
    [SerializeField] protected CanvasGroup main;

    [SerializeField] protected Button btnClose;

    public bool isShow;

    protected virtual void Awake()
    {
        if (btnClose != null) btnClose.onClick.AddListener(Hide);
    }

    public virtual void Show()
    {
        Tween.StopAll(main.transform);
        main.alpha = 0f;
        isShow = true;
        gameObject.SetActive(true);
        Tween.Alpha(main, 0, 1f, .5f);
    }

    public virtual void Hide()
    {
        Tween.StopAll(main.transform);
        main.alpha = 1f;
        Tween.Alpha(main, 0f, .5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
        isShow = false;
    }


    protected virtual void OnDisable()
    {
        if (btnClose != null)
            Tween.StopAll(btnClose.transform);
        Tween.StopAll(main);
    }
    public virtual void Toggle()
    {
        isShow = !isShow;
        if (isShow)
        {
            Show();
        }
        else Hide();
    }

}