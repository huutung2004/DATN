using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý vùng đất trồng hoa - gắn vào một GameObject đại diện cho vùng đất.
/// Tự động chia grid dựa trên kích thước và số ô.
/// </summary>
public class FlowerPlantingSystem : MonoBehaviour
{
    public static FlowerPlantingSystem Instance;

    [Header("Grid Settings")]
    [SerializeField] private int m_gridRows = 3;
    [SerializeField] private int m_gridCols = 3;
    [SerializeField] private float m_cellSize = 1.2f;

    [Header("Prefabs")]
    [SerializeField] private GameObject m_flowerPreviewPrefab;  
    [SerializeField] private GameObject m_flowerSeedPrefab;      
    [SerializeField] private GameObject m_flowerGrownPrefab;    
    [SerializeField] private GameObject m_rewardPrefab;        

    [Header("Grow Settings")]
    [SerializeField] private float m_growTime = 60f;             
    [SerializeField] private float m_grownScaleMultiplier = 1.2f;
    [SerializeField] private float m_scaleUpDuration = 1.5f;

    [Header("Preview Settings")]
    [SerializeField] private LayerMask m_groundLayer;
    [SerializeField] private Color m_validPlaceColor = new Color(0f, 1f, 0f, 0.4f);
    [SerializeField] private Color m_invalidPlaceColor = new Color(1f, 0f, 0f, 0.4f);

    private PlotCell[,] m_cells;
    private GameObject m_previewObj;
    private Renderer[] m_previewRenderers;
    private PlotCell m_hoveredCell;
    private bool m_isPlantingMode = false;


    private void Awake()
    {
        Instance = this;
        BuildGrid();
    }

    private void Update()
    {
        if (!m_isPlantingMode) return;
        UpdatePreview();

        if (Input.GetMouseButtonDown(0))
            TryPlant();

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            ExitPlantingMode();
    }

    #region Grid Construction

    private void BuildGrid()
    {
        m_cells = new PlotCell[m_gridRows, m_gridCols];

        float totalWidth = m_gridCols * m_cellSize;
        float totalDepth = m_gridRows * m_cellSize;
        Vector3 origin = transform.position
            - transform.right * (totalWidth / 2f - m_cellSize / 2f)
            - transform.forward * (totalDepth / 2f - m_cellSize / 2f);

        for (int r = 0; r < m_gridRows; r++)
        {
            for (int c = 0; c < m_gridCols; c++)
            {
                Vector3 worldPos = origin
                    + transform.right * (c * m_cellSize)
                    + transform.forward * (r * m_cellSize);

                m_cells[r, c] = new PlotCell
                {
                    worldPosition = worldPos,
                    isOccupied = false,
                    row = r,
                    col = c
                };
            }
        }
    }

    #endregion

    #region Planting Mode

    public void EnterPlantingMode()
    {
        if (m_isPlantingMode) return;
        m_isPlantingMode = true;
        SpawnPreview();
    }

    public void ExitPlantingMode()
    {
        m_isPlantingMode = false;
        if (m_previewObj != null)
        {
            Destroy(m_previewObj);
            m_previewObj = null;
        }
        m_hoveredCell = null;
    }

    #endregion

    #region Preview

    private void SpawnPreview()
    {
        if (m_flowerPreviewPrefab == null) return;
        m_previewObj = Instantiate(m_flowerPreviewPrefab);
        m_previewObj.SetActive(false);
        m_previewRenderers = m_previewObj.GetComponentsInChildren<Renderer>();
        SetPreviewColor(m_validPlaceColor);
    }

    private void UpdatePreview()
    {
        if (m_previewObj == null) return;

        PlotCell nearest = GetCellUnderCursor();
        m_hoveredCell = nearest;

        if (nearest == null)
        {
            m_previewObj.SetActive(false);
            return;
        }

        m_previewObj.SetActive(true);
        m_previewObj.transform.position = nearest.worldPosition;
        m_previewObj.transform.rotation = transform.rotation;

        bool canPlace = !nearest.isOccupied;
        SetPreviewColor(canPlace ? m_validPlaceColor : m_invalidPlaceColor);
    }

    private void SetPreviewColor(Color c)
    {
        if (m_previewRenderers == null) return;
        foreach (var r in m_previewRenderers)
        {
            foreach (var mat in r.materials)
            {
                mat.color = c;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
            }
        }
    }

    #endregion

    #region Planting

    private void TryPlant()
    {
        if (m_hoveredCell == null || m_hoveredCell.isOccupied) return;

        if (InventoryManager.Instance == null || !InventoryManager.Instance.HasItem(ItemType.flowerBlue, 1))
        {
            Debug.Log("Không đủ hoa để trồng!");
            return;
        }

        InventoryManager.Instance.ConsumeItem(ItemType.flowerBlue, 1);

        m_hoveredCell.isOccupied = true;
        StartCoroutine(GrowFlowerRoutine(m_hoveredCell));

        if (!InventoryManager.Instance.HasItem(ItemType.flowerBlue, 1))
            ExitPlantingMode();
    }

    private IEnumerator GrowFlowerRoutine(PlotCell cell)
    {
        GameObject seed = null;
        if (m_flowerSeedPrefab != null)
        {
            seed = Instantiate(m_flowerSeedPrefab, cell.worldPosition, transform.rotation);
            seed.transform.localScale = Vector3.one * 0.15f;
        }

        if (seed != null)
            yield return AnimateScale(seed.transform, Vector3.one * 0.15f, Vector3.one * 0.5f, 0.6f);

        float elapsed = 0f;
        while (elapsed < m_growTime)
        {
            elapsed += Time.deltaTime;

            if (seed != null)
            {
                float pulse = 1f + Mathf.Sin(elapsed * 2f) * 0.05f;
                seed.transform.localScale = Vector3.one * 0.5f * pulse;
            }

            yield return null;
        }

        if (seed != null)
            Destroy(seed);

        if (m_flowerGrownPrefab != null)
        {
            GameObject grown = Instantiate(m_flowerGrownPrefab, cell.worldPosition, transform.rotation);
            grown.transform.localScale = Vector3.zero;

            yield return AnimateScale(grown.transform, Vector3.zero, Vector3.one * m_grownScaleMultiplier, m_scaleUpDuration, AnimCurve.EaseOutBack);

            yield return new WaitForSeconds(1f);

            if (ParticalManager.Instance != null)
                ParticalManager.Instance.PlaySomke(cell.worldPosition);

            yield return AnimateScale(grown.transform, grown.transform.localScale, Vector3.zero, 0.4f);
            Destroy(grown);
        }

        if (m_rewardPrefab != null)
        {
            Vector3 spawnPos = cell.worldPosition + Vector3.up * 0.3f;
            GameObject reward = Instantiate(m_rewardPrefab, spawnPos, Quaternion.identity);
            reward.SetActive(true);
            // Bounce nhẹ
            StartCoroutine(BounceReward(reward.transform));
        }

        // Giải phóng ô
        cell.isOccupied = false;
    }

    #endregion

    // -------------------------------------------------------
    #region Helpers

    private PlotCell GetCellUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, m_groundLayer))
            return null;

        PlotCell best = null;
        float bestDist = m_cellSize * 0.6f; // threshold nửa ô

        for (int r = 0; r < m_gridRows; r++)
        {
            for (int c = 0; c < m_gridCols; c++)
            {
                float d = Vector3.Distance(hit.point, m_cells[r, c].worldPosition);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = m_cells[r, c];
                }
            }
        }
        return best;
    }

    private IEnumerator AnimateScale(Transform t, Vector3 from, Vector3 to, float duration, AnimCurve curve = AnimCurve.EaseOutQuad)
    {
        float elapsed = 0f;
        t.localScale = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = EvaluateCurve(curve, progress);
            t.localScale = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }
        t.localScale = to;
    }

    private float EvaluateCurve(AnimCurve curve, float t)
    {
        switch (curve)
        {
            case AnimCurve.EaseOutQuad:
                return 1f - (1f - t) * (1f - t);
            case AnimCurve.EaseOutBack:
                float c1 = 1.70158f;
                float c3 = c1 + 1f;
                return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
            default:
                return t;
        }
    }

    private IEnumerator BounceReward(Transform t)
    {
        Vector3 startPos = t.position;
        Vector3 peakPos = startPos + Vector3.up * 0.8f;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float p = elapsed / duration;
            float arc = Mathf.Sin(p * Mathf.PI);
            t.position = Vector3.Lerp(startPos, peakPos, arc);
            yield return null;
        }
        t.position = startPos;
    }

    private enum AnimCurve { Linear, EaseOutQuad, EaseOutBack }

    #endregion

    // -------------------------------------------------------
    #region Gizmos

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // Vẽ preview grid trong editor
            float totalWidth = m_gridCols * m_cellSize;
            float totalDepth = m_gridRows * m_cellSize;
            Vector3 origin = transform.position
                - transform.right * (totalWidth / 2f - m_cellSize / 2f)
                - transform.forward * (totalDepth / 2f - m_cellSize / 2f);

            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.4f);
            for (int r = 0; r < m_gridRows; r++)
            {
                for (int c = 0; c < m_gridCols; c++)
                {
                    Vector3 pos = origin
                        + transform.right * (c * m_cellSize)
                        + transform.forward * (r * m_cellSize);
                    Gizmos.DrawWireCube(pos, new Vector3(m_cellSize * 0.9f, 0.05f, m_cellSize * 0.9f));
                }
            }
        }
        else if (m_cells != null)
        {
            for (int r = 0; r < m_gridRows; r++)
            {
                for (int c = 0; c < m_gridCols; c++)
                {
                    Gizmos.color = m_cells[r, c].isOccupied
                        ? new Color(1f, 0.3f, 0.3f, 0.4f)
                        : new Color(0.3f, 1f, 0.5f, 0.4f);
                    Gizmos.DrawCube(m_cells[r, c].worldPosition, new Vector3(m_cellSize * 0.85f, 0.04f, m_cellSize * 0.85f));
                }
            }
        }
    }
#endif

    #endregion
}

[System.Serializable]
public class PlotCell
{
    public Vector3 worldPosition;
    public bool isOccupied;
    public int row;
    public int col;
}