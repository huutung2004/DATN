using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] public float m_interactDistance = 5f;
    [SerializeField] private IInteractable m_currentInteract;
    private Camera _camera;
    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        if (float.IsInfinity(mousePos.x) || float.IsNaN(mousePos.x) ||
            float.IsInfinity(mousePos.y) || float.IsNaN(mousePos.y))
            return;

        Vector3 viewportPoint = _camera.ScreenToViewportPoint(mousePos);
        if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
        {
            Clear();
            return;
        }
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * m_interactDistance, Color.red);

        if (Physics.Raycast(ray, out hit, m_interactDistance))
        {
            IInteractable obj = hit.collider.GetComponentInParent<IInteractable>();

            if (obj != null)
            {
                if (obj == m_currentInteract) return;

                if (m_currentInteract != null)
                    m_currentInteract?.UnHolding();

                m_currentInteract = obj;
                m_currentInteract?.Holding();
            }
            else
            {
                Clear();
            }
        }
        else Clear();
    }
    private void Clear()
    {
        if (m_currentInteract != null)
        {
            m_currentInteract?.UnHolding();
            m_currentInteract = null;
        }
    }
    private void OnDrawGizmos()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 mousePos = Input.mousePosition;

        if (float.IsInfinity(mousePos.x) || float.IsNaN(mousePos.x) ||
            mousePos.x < 0 || mousePos.y < 0 || mousePos.x > Screen.width || mousePos.y > Screen.height)
            return;

        try
        {
            Ray ray = cam.ScreenPointToRay(mousePos);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray.origin, ray.direction * m_interactDistance);
        }
        catch { }
    }
}
