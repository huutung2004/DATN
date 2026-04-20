using UnityEngine;
using UnityEngine.EventSystems;
using StarterAssets;

public class UITouchZone : MonoBehaviour, IDragHandler, IPointerUpHandler
{
    [SerializeField] private StarterAssetsInputs _input;
    
    [Tooltip("Độ nhạy xoay")]
    public float sensitivity = 0.15f; 

    public void OnDrag(PointerEventData eventData)
    {
        if (_input.isTouchMode)
        {

            Vector2 inputDelta = new Vector2(eventData.delta.x,- eventData.delta.y);
            _input.LookInput(inputDelta * sensitivity);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _input.LookInput(Vector2.zero);
    }
}