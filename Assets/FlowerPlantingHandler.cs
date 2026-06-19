using UnityEngine;

public class FlowerPlantingHandler : MonoBehaviour
{
    // Gọi từ HotBarManager.SelectHotbar() sau khi equip item.
    // Thêm dòng này vào HotBarManager.SelectHotbar():
    //    FlowerPlantingHandler.Instance?.OnItemSelected(item);

    public static FlowerPlantingHandler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void OnItemSelected(Item item)
    {
        if (item != null && item.m_data.m_type == ItemType.flowerBlue && item.m_data.m_canEquid)
        {
            FlowerPlantingSystem.Instance?.EnterPlantingMode();
        }
        else
        {
            FlowerPlantingSystem.Instance?.ExitPlantingMode();
        }
    }

    public void OnItemDeselected()
    {
        FlowerPlantingSystem.Instance?.ExitPlantingMode();
    }
}