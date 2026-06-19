using System.Collections;
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
    public bool HasItem(ItemType type, int amount = 1)
    {
        return GetItemCount(type) >= amount;
    }
    private void Awake()
    {
        Instance = this;

        m_allSlots.Clear();
        m_allSlots.AddRange(m_slotsHotbar);
        m_allSlots.AddRange(m_slotsInventory);
    }

    private IEnumerator Start()
    {
        foreach (var slot in m_allSlots)
        {
            slot.Clear();
        }
        yield return new WaitForSeconds(0.3f);
        AddItemByType(ItemType.wood, 100);
        AddItemByType(ItemType.stone, 100);
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
                    ReducePop.Instance.FillData($" {amount} {slot.m_currentItem.m_data.m_nameOfItem} ", slot.m_currentItem.m_data.m_spriteRender);
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
    public bool ConsumeItem(ItemType type, int amount = 1)
    {
        if (!HasItem(type, amount)) return false;

        int remaining = amount;

        foreach (var slot in m_allSlots)
        {
            if (remaining <= 0) break;

            if (slot.m_currentItem == null) continue;
            if (slot.m_currentItem.m_data.m_type != type) continue;

            int inSlot = slot.m_currentItem.m_data.m_count;
            if (inSlot <= remaining)
            {
                remaining -= inSlot;
                slot.Clear();
            }
            else
            {
                slot.m_currentItem.m_data.m_count -= remaining;
                remaining = 0;
                slot.UpdateCount();
            }
        }

        UpdateUI();
        return true;
    }
}