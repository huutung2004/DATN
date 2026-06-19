using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class DameDealer : MonoBehaviour
{
    bool canDealDamage;
    List<GameObject> hasDealtDamage;

    [SerializeField] private float _weaponLenght;
    [SerializeField] private float _weaponDamage;
    [SerializeField] private LayerMask layer;

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }

    void Update()
    {
        if (canDealDamage)
        {
            float capsuleRadius = 0.2f;

            Vector3 pointTop = transform.position;
            Vector3 pointBottom = transform.position - transform.up * _weaponLenght; 

            Collider[] hitColliders = Physics.OverlapCapsule(pointTop, pointBottom, capsuleRadius, layer);

            foreach (Collider hit in hitColliders)
            {
                if (!hasDealtDamage.Contains(hit.gameObject))
                {
                    ITakeDamage takeDamageComponent = hit.GetComponent<ITakeDamage>() ?? hit.GetComponentInParent<ITakeDamage>();

                    if (takeDamageComponent != null)
                    {
                        takeDamageComponent.TakeDamage(_weaponDamage);
                        Debug.Log("<color=green>Chém trúng mục tiêu: " + hit.name + "</color>");
                        hasDealtDamage.Add(hit.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("Chạm vào " + hit.name + " (Layer Player) nhưng KHÔNG tìm thấy ITakeDamage!");
                    }
                }
            }
        }
    }

    // Cập nhật lại Gizmos để mô phỏng hình dạng của Capsule
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        Vector3 pointTop = transform.position;
        Vector3 pointBottom = transform.position - transform.up * _weaponLenght;

        // Vẽ trục giữa của thanh kiếm
        Gizmos.DrawLine(pointTop, pointBottom);
        
        // Vẽ 2 hình cầu ở 2 đầu (để hình dung vùng quét của Capsule)
        Gizmos.DrawWireSphere(pointTop, 0.2f);
        Gizmos.DrawWireSphere(pointBottom, 0.2f);
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        Tween.Delay(0.2f, () => canDealDamage = false);
    }
}