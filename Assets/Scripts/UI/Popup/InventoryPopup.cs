using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPopup : BasePopup
{
    public override void Show()
    {
        base.Show();
    }
    public virtual void Toggle()
    {
        if(isShow) Hide();
        else Show();
    }
}
