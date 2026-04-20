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

    private void SelectHotbar(int index)
    {
        if (index < 0 || index >= m_listHotbar.Count) return;
        if (currentIndex != -1 && currentIndex != index)
        {
            m_listHotbar[currentIndex].Deselect();
        }
        currentIndex = index;
        m_listHotbar[index].Select();
    }
}