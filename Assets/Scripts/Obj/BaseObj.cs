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
    public Item item;
    protected virtual void Start()
    {
        if (m_outline != null) m_outline.enabled = false;
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
        if (item)
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
        if (m_outline)
            m_outline.enabled = true;
        m_canInteract = true;
        if (HoldingPopup.Instance)
        {
            if (item)
                HoldingPopup.Instance.FillData(item.m_data.m_nameOfItem, null, GetPromt());
            else HoldingPopup.Instance.FillData(m_nameOfObj, null, GetPromt());
            HoldingPopup.Instance.Show();
        }
    }
    public virtual void UnHolding()
    {
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

}
