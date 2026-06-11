using System.Collections.Generic;
using TMPro;
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
    [Header("Temp UI")]
    [SerializeField] private TMP_Text _wood;
    [SerializeField] private TMP_Text _stone;

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

        // if (m_items.Count > 0)
        // {
        //     foreach (Item item in m_items)
        //     {
        //         AddItem(item);
        //     }
        // }
        UpdateUI();
    }
    //Temp for header
    public void UpdateUI()
    {
        _wood.SetText($"{GetItemCount(ItemType.wood)}");
        _stone.SetText($"{GetItemCount(ItemType.stone)}");
    }
    public void AddItemByType(ItemType type, int amount = 1)
    {

        // Tìm stack đã tồn tại
        foreach (var slot in m_allSlots)
        {
            if (slot.m_currentItem != null &&
                slot.m_currentItem.m_data.m_type == type)
            {
                slot.m_currentItem.m_data.m_count += amount;
                if (amount < 0)
                {
                    ReducePop.Instance.FillData($"- {amount} {slot.m_currentItem.m_data.m_nameOfItem} ", slot.m_currentItem.m_data.m_spriteRender);
                }
                slot.UpdateCount();
                UpdateUI();
                return;
            }
        }


        // Tìm item mẫu
        Item itemPrefab = m_items.Find(x => x.m_data.m_type == type);

        if (itemPrefab == null)
        {
            Debug.LogWarning($"Không tìm thấy item type {type}");
            return;
        }

        Item newItem = Instantiate(itemPrefab);
        newItem.m_data.m_count = amount;

        AddItem(newItem);
    }
    public void AddItem(Item newItem)
    {
        if (newItem == null) return;
        foreach (var slot in m_slotsHotbar)
        {
            if (slot.m_currentItem != null &&
                slot.m_currentItem.m_data.m_nameOfItem == newItem.m_data.m_nameOfItem)
            {
                slot.m_currentItem.m_data.m_count += newItem.m_data.m_count;
                LootPop.Instance.FillData($"{newItem.m_data.m_nameOfItem} {newItem.m_data.m_count}", newItem.m_data.m_spriteRender);

                slot.UpdateCount();
                Destroy(newItem.gameObject);
                UpdateUI();
                return;
            }
        }

        foreach (var slot in m_slotsHotbar)
        {
            if (slot.m_currentItem == null)
            {
                slot.SetItem(newItem);
                LootPop.Instance.FillData($"{newItem.m_data.m_nameOfItem} {newItem.m_data.m_count}", newItem.m_data.m_spriteRender);
                UpdateUI();
                return;
            }
        }

        foreach (var slot in m_slotsInventory)
        {
            if (slot.m_currentItem != null &&
                slot.m_currentItem.m_data.m_nameOfItem == newItem.m_data.m_nameOfItem)
            {
                slot.m_currentItem.m_data.m_count += newItem.m_data.m_count;
                LootPop.Instance.FillData($"{newItem.m_data.m_nameOfItem} {newItem.m_data.m_count}", newItem.m_data.m_spriteRender);

                slot.UpdateCount();
                Destroy(newItem.gameObject);
                UpdateUI();
                return;
            }
        }

        foreach (var slot in m_slotsInventory)
        {
            if (slot.m_currentItem == null)
            {
                slot.SetItem(newItem);
                LootPop.Instance.FillData($"{newItem.m_data.m_nameOfItem} {newItem.m_data.m_count}", newItem.m_data.m_spriteRender);
                UpdateUI();
                return;
            }
        }
        Debug.Log("Inventory Full");
    }
    public int GetItemCount(ItemType type)
    {
        int count = 0;

        foreach (var slot in m_allSlots)
        {
            if (slot.m_currentItem == null)
                continue;

            if (slot.m_currentItem.m_data.m_type == type)
            {
                count += slot.m_currentItem.m_data.m_count;
            }
        }
        return count;
    }
}