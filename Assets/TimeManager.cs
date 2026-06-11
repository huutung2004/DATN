
using System;
using SmnStyleHardline.Demo;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    public static TimeManager Instance;

    [Header("Time Settings")]
    [SerializeField] private bool testMode = false;

    [Tooltip("1 ngày = 15 phút thực")]
    [SerializeField] private float normalDayLength = 900f;

    [Tooltip("1 ngày = 30 giây để test")]
    [SerializeField] private float testDayLength = 30f;

    [Header("Current Time")]
    [Range(0f, 24f)]
    [SerializeField] private float currentHour = 6f;

    [SerializeField] private int currentDay = 1;
    public event Action<int> OnDayChanged;
    public SSHDemoDayNightManager sSHDemoDayNight;
    [SerializeField] private TMP_Text timeText;

    private float DayLength =>
        testMode ? testDayLength : normalDayLength;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        AdvanceTime();

        if (sSHDemoDayNight == null)
            return;

        if (TimeManager.Instance == null)
            return;
        timeText.text =
$"Day {TimeManager.Instance.Day}\n{TimeManager.Instance.GetTimeString()}";

        sSHDemoDayNight.cycleValue =
            TimeManager.Instance.SSHCycleValue;
    }

    private void AdvanceTime()
    {
        float hourPerSecond = 24f / DayLength;

        currentHour += hourPerSecond * Time.deltaTime;

        if (currentHour >= 24f)
        {
            currentHour -= 24f;
            currentDay++;

            OnDayChanged?.Invoke(currentDay);
        }
    }

    public int Hour => Mathf.FloorToInt(currentHour);

    public int Minute =>
        Mathf.FloorToInt((currentHour - Hour) * 60f);

    public int Day => currentDay;

    public float NormalizedTime =>
        currentHour / 24f;

    public string GetTimeString()
    {
        return $"{Hour:00}:{Minute:00}";
    }

    public bool IsMorning =>
        currentHour >= 6f && currentHour < 12f;

    public bool IsAfternoon =>
        currentHour >= 12f && currentHour < 18f;

    public bool IsNight =>
        currentHour >= 18f || currentHour < 6f;
    public float SSHCycleValue
    {
        get
        {
            float h = currentHour;

            if (h >= 6f && h < 12f)
            {
                return Mathf.Lerp(
                    0f,
                    0.25f,
                    (h - 6f) / 6f);
            }

            if (h >= 12f && h < 18f)
            {
                return Mathf.Lerp(
                    0.25f,
                    0.5f,
                    (h - 12f) / 6f);
            }

            if (h >= 18f && h < 24f)
            {
                return Mathf.Lerp(
                    0.5f,
                    0.75f,
                    (h - 18f) / 6f);
            }

            return Mathf.Lerp(
                0.75f,
                1f,
                h / 6f);
        }
    }
}