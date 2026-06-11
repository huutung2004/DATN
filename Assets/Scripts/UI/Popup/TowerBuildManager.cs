using UnityEngine;

public class TowerBuildManager : MonoBehaviour
{
    public static TowerBuildManager Instance;

    [Header("Layer")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask towerLayer;

    [Header("Build Setting")]
    [SerializeField] private float checkRadius = 1f;
    private bool currentCanBuild;

    private TowerBase selectedTowerPrefab;
    private GameObject previewObject;

    private Renderer[] previewRenderers;

    private bool isBuilding;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isBuilding)
            return;
        if (selectedTowerPrefab)
            UpdatePreviewPosition();

        if (Input.GetMouseButtonDown(0))
        {

            PlaceTower();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelBuild();
        }
    }

    public void StartBuild(TowerBase towerPrefab)
    {
        selectedTowerPrefab = towerPrefab;

        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = Instantiate(towerPrefab.gameObject);
        TowerBase previewTower = previewObject.GetComponent<TowerBase>();
        if (previewTower != null)
        {
            previewTower.InitData();
        }
        DisablePreviewComponents(previewObject);

        previewRenderers =
            previewObject.GetComponentsInChildren<Renderer>();

        isBuilding = true;
    }

    private void UpdatePreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            previewObject.transform.position = hit.point;

            currentCanBuild = IsValidPosition();

            SetPreviewColor(currentCanBuild);
        }
    }

    private bool IsValidPosition()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                previewObject.transform.position,
                checkRadius,
                towerLayer);

        return hits.Length == 0;
    }

    private void PlaceTower()
    {
        if (!IsValidPosition())
            return;
        if (InventoryManager.Instance.GetItemCount(ItemType.wood) < previewObject.GetComponent<TowerBase>().CurrentData.woodCost)
        {
            Debug.Log("Không đủ gỗ");
            return;
        }

        if (InventoryManager.Instance.GetItemCount(ItemType.stone) < previewObject.GetComponent<TowerBase>().CurrentData.stoneCost)
        {
            Debug.Log("Không đủ đá");
            return;
        }
        Instantiate(
            selectedTowerPrefab.gameObject,
            previewObject.transform.position,
            Quaternion.identity);
        InventoryManager.Instance.AddItemByType(
    ItemType.wood,
    -previewObject.GetComponent<TowerBase>().CurrentData.woodCost);

        InventoryManager.Instance.AddItemByType(
            ItemType.stone,
            -previewObject.GetComponent<TowerBase>().CurrentData.stoneCost);
        Destroy(previewObject);
        previewObject = null;
        selectedTowerPrefab = null;
        isBuilding = false;


    }

    private void CancelBuild()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = null;
        selectedTowerPrefab = null;

        isBuilding = false;
    }

    private void DisablePreviewComponents(GameObject preview)
    {
        foreach (Collider col in preview.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        foreach (MonoBehaviour mb in preview.GetComponentsInChildren<MonoBehaviour>())
        {
            mb.enabled = false;
        }
    }

    private void SetPreviewColor(bool canBuild)
    {
        Color color =
            canBuild
            ? new Color(0, 1, 0, 0.5f)
            : new Color(1, 0, 0, 0.5f);

        foreach (Renderer renderer in previewRenderers)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.color = color;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (previewObject == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            previewObject.transform.position,
            checkRadius);
    }
    private void OnDrawGizmos()
    {
        if (previewObject == null)
            return;

        Gizmos.color = currentCanBuild
            ? Color.green
            : Color.red;

        Gizmos.DrawWireSphere(
            previewObject.transform.position,
            checkRadius);
    }
}