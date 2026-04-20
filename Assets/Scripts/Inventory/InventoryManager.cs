using System.Collections.Generic;
using UnityEngine;

public interface IInventorySlot
{
    Item GetItem();
    void SetItem(Item item);
    void Clear();
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Init Items")]
    [SerializeField] private List<Item> m_items;

    [Header("Hotbar Slots ")]
    [SerializeField] private List<Slot> m_slotsHotbar;

    [Header("Inventory Slots ")]
    [SerializeField] private List<Slot> m_slotsInventory;

    private List<Slot> m_allSlots = new List<Slot>();

    private void Awake()
    {
        Instance = this;

        m_allSlots.Clear();
        m_allSlots.AddRange(m_slotsHotbar);
        m_allSlots.AddRange(m_slotsInventory);
    }

    private void Start()
    {
        foreach (var slot in m_allSlots)
        {
            slot.Clear();
        }

        if (m_items.Count > 0)
        {
            foreach (Item item in m_items)
            {
                AddItem(item);
            }
        }
    }

    public void AddItem(Item newItem)
    {
        if (newItem == null) return;

   
        foreach (var slot in m_allSlots)
        {
            if (slot.m_currentItem != null &&
                slot.m_currentItem.m_data.m_nameOfItem ==
                newItem.m_data.m_nameOfItem)
            {
                slot.m_currentItem.m_data.m_count += newItem.m_data.m_count;
                slot.UpdateCount();
                Destroy(newItem);
                return;
            }
        }


        foreach (var slot in m_slotsHotbar)
        {
            if (slot.m_currentItem == null)
            {
                slot.SetItem(newItem);

                return;
            }
        }

        foreach (var slot in m_slotsInventory)
        {
            if (slot.m_currentItem == null)
            {
                slot.SetItem(newItem);

                return;
            }
        }
        Debug.Log("Inventory Full");
    }
}