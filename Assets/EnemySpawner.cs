using System.Collections.Generic;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    [System.Serializable]
    public class EnemyType
    {
        [Tooltip("Prefab của loại enemy (phải có Component EnemyBrain)")]
        public Enemy Prefab;

        [Tooltip("Số lượng pre-warm trong pool khi khởi động")]
        public int InitPoolSize = 5;

        [Tooltip("Số lượng tối đa active cùng lúc")]
        public int MaxActive = 10;

        [Tooltip("Trọng số spawn (weight), càng cao càng hay được chọn")]
        [Range(1, 10)]
        public int SpawnWeight = 1;

        [HideInInspector] public ObjectPool<Enemy> Pool;
        [HideInInspector] public List<Enemy> ActiveEnemies = new();
    }

    [Header("Enemy Types (3 loại)")]
    [SerializeField] private EnemyType[] enemyTypes = new EnemyType[3];

    [Header("Spawn Settings")]
    [Tooltip("Danh sách các điểm spawn — enemy sẽ xuất hiện ngẫu nhiên tại một trong các điểm này")]
    [SerializeField] private List<Transform> spawnPoints = new();

    [Tooltip("Số giây giữa mỗi lần spawn một enemy")]
    [SerializeField] private float spawnInterval = 5f;

    [Header("Night / Morning Transition")]
    [Tooltip("Nếu true, fade-despawn tất cả enemy khi trời sáng thay vì tắt ngay")]
    [SerializeField] private bool smoothDespawnOnMorning = true;

    [Tooltip("Khoảng cách delay giữa mỗi enemy khi despawn theo danh sách")]
    [SerializeField] private float despawnStagger = 0.3f;

    private bool _isNightMode = false;
    private float _spawnTimer = 0f;
    private Transform _poolRoot;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _poolRoot = new GameObject("[EnemyPool Root]").transform;
        _poolRoot.SetParent(transform);

        InitializePools();
    }

    private void OnEnable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayChanged += OnDayChanged;
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayChanged -= OnDayChanged;
    }

    private void Update()
    {
        if (TimeManager.Instance == null) return;

        bool nowNight = TimeManager.Instance.IsNight;
        if (nowNight != _isNightMode)
        {
            _isNightMode = nowNight;
            if (_isNightMode)
                OnNightBegin();
            else
                OnMorningBegin();
        }

        if (_isNightMode)
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                _spawnTimer = spawnInterval;
                TrySpawnEnemy();
            }
        }
    }

    private void InitializePools()
    {
        for (int i = 0; i < enemyTypes.Length; i++)
        {
            var et = enemyTypes[i];
            if (et.Prefab == null)
            {
                Debug.LogWarning($"[EnemySpawner] EnemyType[{i}] chưa gán Prefab!");
                continue;
            }

            var sub = new GameObject($"Pool_{et.Prefab.name}").transform;
            sub.SetParent(_poolRoot);

            et.Pool = new ObjectPool<Enemy>(et.Prefab, et.InitPoolSize, sub);
            et.ActiveEnemies = new List<Enemy>();
        }
    }

    private void TrySpawnEnemy()
    {
        EnemyType chosen = PickRandomEnemyType();
        if (chosen == null) return;

        if (chosen.ActiveEnemies.Count >= chosen.MaxActive) return;

        Vector3 pos = GetRandomSpawnPosition();
        Enemy enemy = chosen.Pool.Get();
        enemy.Agent.Warp(pos);
        enemy.transform.position = pos;
        enemy.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        enemy.currentHp = enemy.maxHp;

        chosen.ActiveEnemies.Add(enemy);
    }

    private EnemyType PickRandomEnemyType()
    {
        int totalWeight = 0;
        foreach (var et in enemyTypes)
            if (et.Prefab != null) totalWeight += et.SpawnWeight;

        if (totalWeight == 0) return null;

        int roll = Random.Range(0, totalWeight);
        int acc = 0;
        foreach (var et in enemyTypes)
        {
            if (et.Prefab == null) continue;
            acc += et.SpawnWeight;
            if (roll < acc) return et;
        }
        return null;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] Danh sách spawnPoints trống! Dùng vị trí spawner.");
            return transform.position;
        }

        var valid = spawnPoints.FindAll(t => t != null);
        if (valid.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] Tất cả spawnPoints đều null!");
            return transform.position;
        }

        return valid[Random.Range(0, valid.Count)].position;
    }
    public void ReturnEnemy(Enemy enemy)
    {
        foreach (var et in enemyTypes)
        {
            if (et.ActiveEnemies.Remove(enemy))
            {
                et.Pool.ReturnToPool(enemy);
                return;
            }
        }
        enemy.gameObject.SetActive(false);
    }
    private void OnNightBegin()
    {
        Debug.Log("[EnemySpawner]  Đêm bắt đầu — Enemy sẽ xuất hiện.");
        _spawnTimer = 0f;
    }

    private void OnMorningBegin()
    {
        Debug.Log("[EnemySpawner]  Sáng bắt đầu — Despawn tất cả enemy.");
        if (smoothDespawnOnMorning)
            StartCoroutine(DespawnAllStaggered());
        else
            DespawnAllImmediate();
    }

    private void DespawnAllImmediate()
    {
        foreach (var et in enemyTypes)
        {
            var copy = new List<Enemy>(et.ActiveEnemies);
            foreach (var e in copy)
                et.Pool.ReturnToPool(e);
            et.ActiveEnemies.Clear();
        }
    }

    private System.Collections.IEnumerator DespawnAllStaggered()
    {
        var all = new List<Enemy>();
        foreach (var et in enemyTypes)
            all.AddRange(et.ActiveEnemies);

        for (int i = all.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (all[i], all[j]) = (all[j], all[i]);
        }

        foreach (var e in all)
        {
            if (e == null || !e.gameObject.activeSelf) continue;
            ReturnEnemy(e);
            yield return new WaitForSeconds(despawnStagger);
        }
    }
    private void OnDayChanged(int newDay)
    {
        Debug.Log($"[EnemySpawner] Ngày mới: {newDay}");
    }
    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.9f);
        foreach (var pt in spawnPoints)
        {
            if (pt == null) continue;
            Gizmos.DrawWireSphere(pt.position, 0.5f);
            Gizmos.DrawLine(pt.position, pt.position + Vector3.up * 2f);
        }
    }
}