using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IInventorySlot,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image m_imgRender;
    [SerializeField] private TMP_Text m_count;

    public static Slot draggingSlot;

    private Transform originalParent;
    private Transform canvasRoot;

    [SerializeField] public CanvasGroup canvasGroup;

    public Item m_currentItem;
    private bool isDroppedSuccessfully;

    public Item GetItem() => m_currentItem;

    private void Start()
    {
        canvasRoot = CanvasManager.canvasManager.transform;
    }

    public void SetItem(Item item)
    {
        m_currentItem = item;

        if (item == null)
        {
            Debug.Log($"[{name}] SetItem NULL -> Clear");

            Clear();
            return;
        }

        Debug.Log($"[{name}] SetItem: {item.m_data.m_nameOfItem} x{item.m_data.m_count}");

        m_imgRender.sprite = item.m_data.m_spriteRender;
        m_count.SetText($"{item.m_data.m_count}");

        Show();
    }

    public void Clear()
    {
        Debug.Log($"[{name}] Clear Slot");

        m_currentItem = null;
        m_imgRender.sprite = null;

        Hide();
    }

    private void Show()
    {
        Debug.Log($"[{name}] Show UI");

        m_imgRender.enabled = true;
        m_count.enabled = true;
    }

    private void Hide()
    {
        Debug.Log($"[{name}] Hide UI");

        m_imgRender.enabled = false;
        m_count.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_currentItem == null)
        {
            Debug.Log($"[{name}] Không có item -> Không drag");
            return;
        }

        Debug.Log($"[{name}] Begin Drag: {m_currentItem.m_data.m_nameOfItem}");

        draggingSlot = this;

        originalParent = transform.parent;

        Debug.Log($"Original Parent: {originalParent.name}");

        transform.SetParent(canvasRoot.transform);

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[{name}] End Drag");

        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;

        canvasGroup.blocksRaycasts = true;
        if (!isDroppedSuccessfully)
        {
            Debug.Log("Drop Fail");

            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }

        draggingSlot = null;
    }
    public void UpdateCount()
    {
        if (m_currentItem != null)
        {
            Debug.Log(
                $"[{name}] UpdateCount: {m_currentItem.m_data.m_nameOfItem} x{m_currentItem.m_data.m_count}"
            );

            m_count.SetText($"{m_currentItem.m_data.m_count}");
        }
    }
}