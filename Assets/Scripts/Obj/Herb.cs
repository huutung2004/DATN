using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using StarterAssets;
using UnityEngine;

public class Herb : BaseObj
{
    public override string GetPromt() => "Gather";

    public override void Interact()
    {
        if (isInteracting) return;
        isInteracting = true;
        Vector3 lookPos = transform.position - ThirdPersonController.Instance.transform.position;
        lookPos.y = 0;
        ThirdPersonController.Instance._playerInput.enabled = false;
        if (lookPos != Vector3.zero)
        {
            ThirdPersonController.Instance.transform.rotation = Quaternion.LookRotation(lookPos);
        }

        Tween.Position(ThirdPersonController.Instance.transform, transform.position, 0.5f, Ease.OutQuad)
            .OnComplete(() =>
            {
                ThirdPersonController.Instance._animator.CrossFadeInFixedTime("Pick Fruit 0", 0.1f);
                Tween.Scale(transform, 0.8f, 0.5f, Ease.InOutCirc, 3, CycleMode.Yoyo);
                Tween.Delay(1.5f).OnComplete(() =>
                {
                    ParticalManager.Instance.PlaySomke(transform.position);
                    InventoryManager.Instance.AddItem(item);
                    isInteracting = false;
                    gameObject.SetActive(false);
                    ThirdPersonController.Instance._playerInput.enabled = true;
                });
            });
    }
}
