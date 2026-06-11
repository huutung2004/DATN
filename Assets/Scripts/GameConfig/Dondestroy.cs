using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dondestroy : MonoBehaviour
{
    public static Dondestroy Instance;
    private void Awake()
    {
        
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 60;
        Debug.Log("Run with 60 FPS");
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
