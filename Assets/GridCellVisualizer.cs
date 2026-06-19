using UnityEngine;
[RequireComponent(typeof(FlowerPlantingSystem))]
public class GridCellVisualizer : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Material m_lineMaterial;
    [SerializeField] private Color m_lineColor = new Color(0.3f, 1f, 0.5f, 0.7f);
    [SerializeField] private float m_lineWidth = 0.03f;
    [SerializeField] private float m_heightOffset = 0.02f;

    private LineRenderer[] m_lines;
    private FlowerPlantingSystem m_system;

    [Header("Mirror Grid Settings - must match FlowerPlantingSystem")]
    [SerializeField] private int m_gridRows = 3;
    [SerializeField] private int m_gridCols = 3;
    [SerializeField] private float m_cellSize = 1.2f;

    private void Awake()
    {
        m_system = GetComponent<FlowerPlantingSystem>();
        BuildLines();
        SetVisible(false);
    }

    private void BuildLines()
    {
        int lineCount = (m_gridCols + 1) + (m_gridRows + 1);
        m_lines = new LineRenderer[lineCount];

        float totalW = m_gridCols * m_cellSize;
        float totalD = m_gridRows * m_cellSize;

        Vector3 originCorner = transform.position
            - transform.right * totalW * 0.5f
            - transform.forward * totalD * 0.5f
            + Vector3.up * m_heightOffset;

        int idx = 0;

        for (int r = 0; r <= m_gridRows; r++)
        {
            Vector3 start = originCorner + transform.forward * (r * m_cellSize);
            Vector3 end = start + transform.right * totalW;
            m_lines[idx++] = CreateLine(start, end);
        }

        for (int c = 0; c <= m_gridCols; c++)
        {
            Vector3 start = originCorner + transform.right * (c * m_cellSize);
            Vector3 end = start + transform.forward * totalD;
            m_lines[idx++] = CreateLine(start, end);
        }
    }

    private LineRenderer CreateLine(Vector3 start, Vector3 end)
    {
        GameObject go = new GameObject("GridLine");
        go.transform.SetParent(transform);

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = m_lineWidth;
        lr.endWidth = m_lineWidth;
        lr.material = m_lineMaterial != null ? m_lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startColor = m_lineColor;
        lr.endColor = m_lineColor;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return lr;
    }

    public void SetVisible(bool visible)
    {
        if (m_lines == null) return;
        foreach (var lr in m_lines)
            if (lr != null) lr.enabled = visible;
    }

    public void OnEnterPlantingMode() => SetVisible(true);
    public void OnExitPlantingMode() => SetVisible(false);
}