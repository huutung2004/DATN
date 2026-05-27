using System.Collections.Generic;
using UnityEngine;

public class PlayerEquippedItem : MonoBehaviour
{
    public static PlayerEquippedItem Instance;

    [SerializeField] private Transform m_posItem;

    private Dictionary<BaseObj, BaseObj> m_spawnedCache = new Dictionary<BaseObj, BaseObj>();

    private BaseObj m_currentActiveObj;
    public bool m_isEquidWeapon = false;


    private void Awake()
    {
        Instance = this;
    }

    public void EquipItem(BaseObj prefab)
    {
        if (prefab == null) return;

        if (m_currentActiveObj != null && m_spawnedCache.ContainsKey(prefab) && m_currentActiveObj == m_spawnedCache[prefab])
        {
            m_currentActiveObj.gameObject.SetActive(true);
            if (m_currentActiveObj as BaseWeakpon)
            {
                m_isEquidWeapon = true;
            }
            return;
        }

        UnEquipItem();

        if (m_spawnedCache.ContainsKey(prefab))
        {
            m_currentActiveObj = m_spawnedCache[prefab];
        }
        else
        {
            m_currentActiveObj = Instantiate(prefab, m_posItem);
            m_currentActiveObj.m_inInventory = true;
            m_currentActiveObj.TurnOffOutline();
            m_spawnedCache.Add(prefab, m_currentActiveObj);
            Vector3 originalScale = m_currentActiveObj.transform.localScale;
            m_currentActiveObj.transform.localScale = originalScale * 0.02f;
        }

        m_currentActiveObj.transform.localPosition = Vector3.zero;
        m_currentActiveObj.transform.localRotation = Quaternion.identity;
        if (m_currentActiveObj as BaseWeakpon)
        {
            m_isEquidWeapon = true;
        }
        m_currentActiveObj.gameObject.SetActive(true);
    }

    public void UnEquipItem()
    {
        if (m_currentActiveObj != null)
        {
            if (m_currentActiveObj as BaseWeakpon)
            {
                m_isEquidWeapon = false;
            }
            m_currentActiveObj.gameObject.SetActive(false);
            m_currentActiveObj = null;

        }

    }
    public void StartDealDamage()
    {
        if (m_currentActiveObj as BaseWeakpon)
        {
            var dameDealer = m_currentActiveObj.GetComponentInChildren<DameDealer>();
            if (dameDealer)
            {
                dameDealer.StartDealDamage();
            }
        }
    }
    public void EndDealDamage()
    {
        if (m_currentActiveObj as BaseWeakpon)
        {
            var dameDealer = m_currentActiveObj.GetComponentInChildren<DameDealer>();
            if (dameDealer)
            {
                dameDealer.EndDealDamage();
            }
        }
    }
}