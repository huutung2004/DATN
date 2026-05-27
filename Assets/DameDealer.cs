using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DameDealer : MonoBehaviour
{
    bool canDealDamage;
    List<GameObject> hasDealtDamage;

    [SerializeField] private float _weaponLenght;
    [SerializeField] private float _weaponDamage;

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();    
    }

    void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;
            int  layerMask = 1<<9;
            if(Physics.Raycast(transform.position,-transform.up,out hit , _weaponLenght, layerMask))
            {
                if (!hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    Debug.Log("damage");
                    hasDealtDamage.Add(hit.transform.gameObject);
                }
            }
        }
    }
    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }
    public void EndDealDamage()
    {
        canDealDamage = false;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,transform.position - transform.up*_weaponLenght);
    }

}
