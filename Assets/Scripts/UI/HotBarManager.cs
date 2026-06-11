using System.Collections.Generic;
using UnityEngine;

public class HotBarManager : MonoBehaviour
{
    [SerializeField] private List<HotBar> m_listHotbar;
    private int currentIndex = -1;

    private void Update()
    {
        for (int i = 0; i < m_listHotbar.Count && i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectHotbar(i);
            }
        }
    }

    public void SelectHotbar(int index)
    {
        if (index < 0 || index >= m_listHotbar.Count) return;

        if (currentIndex == index)
        {
            m_listHotbar[currentIndex].Deselect();
            PlayerEquippedItem.Instance.UnEquipItem();
            currentIndex = -1;
            return;
        }

        if (currentIndex != -1)
        {
            m_listHotbar[currentIndex].Deselect();
        }

        currentIndex = index;
        m_listHotbar[currentIndex].Select();

        Item item = m_listHotbar[currentIndex].GetItemInSlot();
        // Debug.Log(item, item.m_data.m_obj);
        if (item != null && item.m_data.m_obj != null && item.m_data.m_canEquid)
        {
            PlayerEquippedItem.Instance.EquipItem(item.m_data.m_obj);
        }
        else
        {
            PlayerEquippedItem.Instance.UnEquipItem();
        }
    }
}