using UnityEngine;
using UnityEngine.UI;

public class FenceBuildManager : MonoBehaviour
{
    public static FenceBuildManager Instance;

    [Header("Layer")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask fenceLayer;

    [Header("Build Setting")]
    [SerializeField] private float rotateSpeed = 90f;

    [Tooltip("0.9 = cho phép sát nhau hơn, 1 = collider thật")]
    [SerializeField] private float overlapMultiplier = 0.9f;

    [SerializeField] private Button startBuildButton;

    public Fence fencePrefab;

    private GameObject previewObject;
    private Renderer[] previewRenderers;

    private bool isBuilding;
    private bool canBuild;

    private float currentYRotation;

    private void Awake()
    {
        Instance = this;

        if (startBuildButton != null)
        {
            startBuildButton.onClick.AddListener(() =>
            {
                StartBuild(fencePrefab);
            });
        }
    }

    private void Update()
    {
        if (!isBuilding)
            return;

        UpdatePreviewPosition();

        HandleRotation();

        if (Input.GetMouseButtonDown(0))
        {
            BuildFence();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelBuild();
        }
    }

    public void StartBuild(Fence prefab)
    {
        if (prefab == null)
            return;

        fencePrefab = prefab;

        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = Instantiate(prefab.gameObject);

        DisablePreviewComponents(previewObject);

        previewRenderers =
            previewObject.GetComponentsInChildren<Renderer>();

        currentYRotation = 0;

        isBuilding = true;

        SetPreviewColor(true);
    }

    private void UpdatePreviewPosition()
    {
        Ray ray =
            Camera.main.ScreenPointToRay(
                Input.mousePosition);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            1000f,
            groundLayer))
        {
            previewObject.transform.position =
                hit.point;

            canBuild = IsValidPosition();

            SetPreviewColor(canBuild);
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.R))
        {
            currentYRotation +=
                rotateSpeed * Time.deltaTime;

            previewObject.transform.rotation =
                Quaternion.Euler(
                    0,
                    currentYRotation,
                    0);
        }
    }

    private bool IsValidPosition()
    {
        BoxCollider box =
            previewObject.GetComponent<BoxCollider>();

        if (box == null)
        {
            Debug.LogWarning(
                "Fence prefab cần BoxCollider");

            return true;
        }

        Vector3 center =
            box.bounds.center;

        Vector3 halfExtents =
            box.bounds.extents * overlapMultiplier;

        Collider[] hits =
            Physics.OverlapBox(
                center,
                halfExtents,
                previewObject.transform.rotation,
                fenceLayer);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject != previewObject)
            {
                return false;
            }
        }

        return true;
    }

    private void BuildFence()
    {
        if (!canBuild)
            return;

        Fence data =
            previewObject.GetComponent<Fence>();

        if (InventoryManager.Instance.GetItemCount(ItemType.wood)
            < data.WoodCost)
        {
            Debug.Log("Không đủ gỗ");
            return;
        }

        Instantiate(
            fencePrefab.gameObject,
            previewObject.transform.position,
            previewObject.transform.rotation);

        InventoryManager.Instance.AddItemByType(
            ItemType.wood,
            -data.WoodCost);

        Destroy(previewObject);

        previewObject = null;

        isBuilding = false;
    }

    private void CancelBuild()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = null;

        isBuilding = false;
    }

    private void DisablePreviewComponents(GameObject obj)
    {
        foreach (Collider col in obj.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        foreach (MonoBehaviour mb in obj.GetComponentsInChildren<MonoBehaviour>())
        {
            mb.enabled = false;
        }
    }

    private void SetPreviewColor(bool valid)
    {
        Color color =
            valid
            ? new Color(0, 1, 0, 0.5f)
            : new Color(1, 0, 0, 0.5f);

        foreach (Renderer renderer in previewRenderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", color);

                if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", color);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (previewObject == null)
            return;

        BoxCollider box =
            previewObject.GetComponent<BoxCollider>();

        if (box == null)
            return;

        Gizmos.color =
            canBuild
            ? Color.green
            : Color.red;

        Matrix4x4 oldMatrix = Gizmos.matrix;

        Gizmos.matrix =
            Matrix4x4.TRS(
                box.bounds.center,
                previewObject.transform.rotation,
                Vector3.one);

        Gizmos.DrawWireCube(
            Vector3.zero,
            box.bounds.size * overlapMultiplier);

        Gizmos.matrix = oldMatrix;
    }
}