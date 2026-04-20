using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager canvasManager;
    private void Awake()
    {
        canvasManager = this;
    }
}
