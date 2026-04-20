using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandle : MonoBehaviour,IDropHandler
{
     [SerializeField] private Slot currentSlot;
    public void OnDrop(PointerEventData eventData)
    {
        Slot draggedSlot = eventData.pointerDrag?.GetComponent<Slot>();

        Debug.Log("OnDrop chạy tại: " + gameObject.name);

        if (draggedSlot == null)
        {
            Debug.Log("Dragged Slot NULL");
            return;
        }

        if (draggedSlot == currentSlot)
        {
            return;
        }

        HandleDrop(draggedSlot);
    }

    private void HandleDrop(Slot source)
    {
        Item sourceItem = source.m_currentItem;
        Item targetItem = currentSlot.m_currentItem;

        if (sourceItem == null) return;

        if (targetItem == null)
        {
            currentSlot.SetItem(sourceItem);
            source.Clear();

            Debug.Log("Move vào ô trống");
            return;
        }

        if (targetItem.m_data.m_nameOfItem ==
            sourceItem.m_data.m_nameOfItem)
        {
            targetItem.m_data.m_count += sourceItem.m_data.m_count;

            currentSlot.SetItem(targetItem);
            source.Clear();

            Debug.Log("Gộp stack");
            return;
        }

        source.SetItem(targetItem);
        currentSlot.SetItem(sourceItem);

        Debug.Log("Swap item");
    }
}
