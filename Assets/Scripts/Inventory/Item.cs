using System.Collections;
using System.Collections.Generic;
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
    none,
    none2,
    none3
}
public interface IAction
{
    public void Action();
}
public class Item : MonoBehaviour
{

    public ItemData m_data;
    public IAction m_action;

    public void TryAction()
    {
        m_action?.Action();
    }
    
}
