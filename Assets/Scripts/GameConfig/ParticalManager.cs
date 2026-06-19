using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public class ParticalManager : MonoBehaviour
{
    public static ParticalManager Instance;
    public GameObject m_smoke;
    public GameObject m_hurtEffect;
    public GameObject m_deathEffect;
    public GameObject m_plantEffect;
    public GameObject m_heartObj;
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
    public void PlayEffect(GameObject go, Vector3 pos)
    {
        go.transform.position = pos;
        go.SetActive(true);
        Tween.Delay(1f, () =>
        {
            go.SetActive(false);
        });
    }

}
