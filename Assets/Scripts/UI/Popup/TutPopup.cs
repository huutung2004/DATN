using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutPopup : BasePopup
{
    [SerializeField] protected RectTransform m_arrowRect;
    public override void Toggle()
    {
        base.Toggle();
        if (isShow)
        {
            m_arrowRect.localEulerAngles = new Vector3(0,0,-90f);
        }
        else m_arrowRect.localEulerAngles = new Vector3(0,0,90f);
    }
}
