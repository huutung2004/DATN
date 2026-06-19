using UnityEngine;

public class Crop : MonoBehaviour
{
    [Header("Crop Settings")]
    public float growTime = 60f;      
    public bool isWatered = false;    

    private float currentGrowTime = 0f;
    private bool isMature = false;

    [Header("Visual")]
    public SpriteRenderer cropRenderer;
    public Sprite seedSprite;
    public Sprite matureSprite;

    private void Start()
    {
        cropRenderer.sprite = seedSprite;
    }

    private void Update()
    {
        if (isMature || !isWatered)
            return;

        currentGrowTime += Time.deltaTime;

        if (currentGrowTime >= growTime)
        {
            GrowUp();
        }
    }

    public void WaterCrop()
    {
        isWatered = true;
        Debug.Log("Crop watered.");
    }

    private void GrowUp()
    {
        isMature = true;
        cropRenderer.sprite = matureSprite;

        Debug.Log("Crop is ready to harvest.");
    }

    public void Harvest()
    {
        if (!isMature)
            return;

        Debug.Log("Harvest successful.");

        Destroy(gameObject);
    }
}