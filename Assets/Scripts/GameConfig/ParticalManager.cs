using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class ParticalManager : MonoBehaviour
{
    public static ParticalManager Instance;
    public GameObject m_smoke;
    private void Awake()
    {
        Instance = this;
        m_smoke.SetActive(false);
    }
    public void PlaySomke(Vector3 pos)
    {
        m_smoke.transform.position = pos;
        m_smoke.SetActive(true);
        Tween.Delay(1f, () =>
        {
            m_smoke.SetActive(false);
        });
    }

}
