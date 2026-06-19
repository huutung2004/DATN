using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinerPopup : BasePopup
{
    public static WinerPopup Instance;
    public Button buttonExit;
    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        buttonExit.onClick.AddListener(Exit);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
