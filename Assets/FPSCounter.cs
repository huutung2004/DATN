using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText;

    private float timer;
    private int frameCount;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        frameCount++;
        timer += Time.unscaledDeltaTime;

        if (timer >= 0.5f)
        {
            int fps = Mathf.RoundToInt(frameCount / timer);
            fpsText.SetText("FPS: {0}", fps);

            frameCount = 0;
            timer = 0f;
        }
    }
}