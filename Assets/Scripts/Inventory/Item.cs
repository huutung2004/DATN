using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct ItemData
{
    public string m_nameOfItem;
    public Sprite m_spriteRender;
    public int m_count;
    public ItemType m_type;
    public bool m_inHotbar;

}
public enum ItemType
{
    wood,
    stone,
    flowerRed,
    flowerBlue,
    MushroomRed,
    MushroomYellow,
    Herb,
    magic,
    sword,
    axe
}
public interface IAction
{
    public void Action();
}
public class Item : MonoBehaviour
{

    public ItemData m_data;
    public SO_Item m_SO;
    public ItemType type;
    private void Awake()
    {
        m_data = m_SO.items.FirstOrDefault(i => i.m_type == type);
    }
    public IAction m_action;

    public void TryAction()
    {
        m_action?.Action();
    }
    
}
