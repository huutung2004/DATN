using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerBase : BaseObj, ITakeDamage
{
    [Header("Tower Config")]
    public TowerType towerType;
    public MeshFilter _meshStand;
    public MeshFilter _meshGun;

    [SerializeField]
    private TowerData towerData;
    [SerializeField]
    private SphereCollider rangeCollider;
    [SerializeField]
    private PolygonArsenal.PolygonBeamStatic beam;
    private TowerEntry towerEntry;
    private int currentLevel;

    private float attackTimer;
    public float currentHp;
    private Enemy currentTarget;
    public Sprite sprite;

    private readonly List<Enemy> enemiesInRange = new();

    public TowerLevelData CurrentData
    {
        get
        {
            if (towerEntry.levels == null || towerEntry.levels.Length == 0)
            {
                Debug.LogError($"Tower {towerType} chưa được cấu hình list Levels trong ScriptableObject!");
                return null;
            }
            // Đảm bảo index không vượt quá độ dài mảng
            int index = Mathf.Clamp(currentLevel, 0, towerEntry.levels.Length - 1);
            return towerEntry.levels[index];
        }
    }
    public override string GetPromt() => "Mouse Click";

    private void Awake()
    {
        InitData();
    }
    public void InitData()
    {
        towerEntry = towerData.towerEntries.Find(
            e => e.towerType == towerType);

        if (towerEntry.IsUnityNull())
        {
            Debug.LogError($"TowerData does not contain entry for {towerType}");
            enabled = false;
            return;
        }
        UpdateData();
        UpdateRange();
    }

    protected override void Update()
    {
        if (m_canInteract)
        {
            if (Input.GetMouseButton(0))
            {
                Interact();
            }
        }
        CleanupEnemies();
        currentTarget = GetClosestEnemy();
        UpdateBeam();
    }
    public override void Interact()
    {
        if (TowerOverviewPop.Instance)
        {
            TowerOverviewPop.Instance.FillData(this);
            TowerOverviewPop.Instance.Show();
        }

    }
    private void UpdateBeam()
    {
        if (beam == null)
            return;

        if (currentTarget == null)
        {
            beam.target = null;
            beam.gameObject.SetActive(false);
            return;
        }

        beam.gameObject.SetActive(true);

        beam.target = currentTarget.transform;

        Attack();
    }
    private void CleanupEnemies()
    {
        enemiesInRange.RemoveAll(enemy =>
            enemy == null ||
            !enemy.gameObject.activeInHierarchy);
    }

    private Enemy GetClosestEnemy()
    {
        Enemy closest = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    private void Attack()
    {
        if (currentTarget == null)
            return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= CurrentData.attackSpeed)
        {
            attackTimer -= CurrentData.attackSpeed;

            Shoot(currentTarget);
        }
    }

    private void Shoot(Enemy target)
    {
        target.TakeDamage(CurrentData.damage);
    }

    public void AddEnemy(Enemy enemy)
    {
        if (enemy == null)
            return;

        if (!enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemiesInRange.Remove(enemy);
    }

    public void Upgrade()
    {
        if (currentLevel >= towerEntry.levels.Length - 1)
            return;

        currentLevel++;
        UpdateRange();
        UpdateData();
        Debug.Log($"{towerType} upgraded to level {currentLevel + 1}");
    }
    private void UpdateData()
    {
        currentHp = CurrentData.heal;
        _meshGun.mesh = CurrentData.mesh_Gun;
        _meshStand.mesh = CurrentData.mesh_Stand;
    }
    private void UpdateRange()
    {
        if (rangeCollider != null)
        {
            rangeCollider.radius = CurrentData.range;
        }
    }
    public override void Holding()
    {
        if (m_outline)
            m_outline.enabled = true;
        m_canInteract = true;
        if (HealBarPopup.Instance)
        {
            HealBarPopup.Instance.FillData(m_nameOfObj, sprite, currentHp / CurrentData.heal);
            HealBarPopup.Instance.Show();
        }

    }
    public override void UnHolding()
    {
        base.UnHolding();
        if (HealBarPopup.Instance)
        {
            HealBarPopup.Instance.Hide();
        }
    }
    public virtual void ReturnPool()
    {
        InventoryManager.Instance.AddItemByType(ItemType.wood, CurrentData.reWood);
        InventoryManager.Instance.AddItemByType(ItemType.stone, CurrentData.reStone);

        Destroy(gameObject);
    }

    public void TakeDamage(float damge)
    {
        CurrentData.heal -= damge;
        HealBarPopup.Instance.GetImage().fillAmount = currentHp / CurrentData.heal;

        if (CurrentData.heal <= 0)
        {
            Destroy(gameObject);
            ParticalManager.Instance.PlaySomke(gameObject.transform.position + Vector3.up * 0.6f);
        }
    }
}