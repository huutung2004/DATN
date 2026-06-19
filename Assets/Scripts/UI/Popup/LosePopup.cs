using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LosePopup : BasePopup
{
    public static LosePopup Instance;
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
