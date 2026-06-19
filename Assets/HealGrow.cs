using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealGrow : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           var heal =  other.GetComponent<PlayerHeal>();
            heal.TakeDamage(-5f);
            gameObject.SetActive(false);
        }
    }
}
