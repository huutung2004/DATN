using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class BaseObj : MonoBehaviour, IInteractable
{
    [SerializeField] protected Outline m_outline;
    public bool m_canInteract = false;
    protected bool isInteracting = false;

    public virtual string GetPromt() => "Interact";
    public string m_nameOfObj;
    public Sprite _icon;
    public Item item;
    public bool m_inInventory = false;
    public GameObject m_particalLoot;

    protected virtual void Start()
    {
        if (m_outline != null) m_outline.enabled = false;
        IdleObj();
    }
    public virtual void Interact()
    {
        if (isInteracting) return;
        isInteracting = true;
        TryTakeItem();
    }
    protected virtual void Update()
    {
        if (m_canInteract)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }
    }
    protected virtual void TryTakeItem()
    {
        if (item && !m_inInventory)
        {
            Debug.Log($"take obj: {gameObject.name}");
            InventoryManager.Instance.AddItem(item);
            EffectInteract();
            gameObject.SetActive(false);
            isInteracting = false;
        }
    }
    public virtual void Holding()
    {
        if (m_inInventory) return;
        if (m_outline)
            m_outline.enabled = true;
        m_canInteract = true;
        ShowHoldingPopup();
    }
    public virtual void ShowHoldingPopup()
    {
        if (HoldingPopup.Instance)
        {
            if (item != null)
            {
                HoldingPopup.Instance.FillData(item.m_data.m_nameOfItem, null, GetPromt());
            }
            else
                HoldingPopup.Instance.FillData(m_nameOfObj, _icon, GetPromt());
            HoldingPopup.Instance.Show();
        }
    }
    public virtual void UnHolding()
    {
        if (m_inInventory) return;
        if (m_outline)
            m_outline.enabled = false;
        m_canInteract = false;
        if (HoldingPopup.Instance)
            HoldingPopup.Instance.Hide();

    }
    protected virtual void EffectInteract()
    {
        if (ParticalManager.Instance)
            ParticalManager.Instance.PlaySomke(gameObject.transform.position);
    }
    protected virtual void IdleObj()
    {
        if (m_inInventory)
        {
            if (m_particalLoot) m_particalLoot.SetActive(false);
            return;
        }
    }
    public virtual void TurnOffOutline()
    {
        if (m_outline)
            m_outline.enabled = false;
    }

}
