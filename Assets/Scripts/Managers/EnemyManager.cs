using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Spawn Table")]
    [SerializeField] private EnemySpawnTable spawnTable;

    [Header("Spawners")]
    [SerializeField] private List<GameObject> spawnerList;

    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Spawn Rate")]
    [SerializeField]
    [Tooltip("Enemies per second")]
    private float baseSpawnRate = 1.2f;
    [SerializeField] private float spawnRateGrowth = 0.05f;

    [Header("Spawn Count Curve")]
    [Tooltip("X = minutes, Y = enemies per spawn tick")]
    [SerializeField] private AnimationCurve enemiesPerTickCurve;

    [Header("Spawner Priority")]
    [Tooltip("higher = flatter distribution")]
    [SerializeField] private float spawnerDistanceBias = 2f;

    [Header("Enemy Cap")]
    [SerializeField] private int maxAliveEnemies = 100;

    private float spawnTimer;
    private double timeSinceStartPlaying;

    private readonly HashSet<BaseEnemy> activeEnemies = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        #region Spawn Enemies

        if (!GameManager.Instance.IsPlaying())
            return;

        timeSinceStartPlaying += Time.deltaTime;

        if (activeEnemies.Count >= maxAliveEnemies)
            return;

        float minutes = (float)timeSinceStartPlaying / 60f;

        float spawnRate =
            baseSpawnRate +
            minutes * spawnRateGrowth;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer > 0f)
            return;

        int spawnCount = Mathf.CeilToInt(
            enemiesPerTickCurve.Evaluate(minutes)
        );

        for (int i = 0; i < spawnCount; i++)
        {
            if (activeEnemies.Count >= maxAliveEnemies)
                break;

            SpawnFromSpawner(minutes);
        }

        spawnTimer = 1f / spawnRate;

        #endregion
    }

    // -------- SPAWN --------
    private void SpawnFromSpawner(float minutes)
    {
        BaseEnemy prefab = spawnTable.GetRandomEnemy(minutes);
        if (prefab == null)
            return;

        GameObject spawner = GetWeightedSpawner();
        if (spawner == null)
            return;

        SpawnEnemy(prefab, spawner.transform.position);
    }

    private GameObject GetWeightedSpawner()
    {
        Vector3 playerPos = player.transform.position;

        float totalWeight = 0f;

        // First pass: calculate total weight
        foreach (var spawner in spawnerList)
        {
            float sqrDist = (playerPos - spawner.transform.position).sqrMagnitude;
            float weight = 1f / (sqrDist + spawnerDistanceBias);

            totalWeight += weight;
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.value * totalWeight;

        // Second pass: pick spawner
        foreach (var spawner in spawnerList)
        {
            float sqrDist = (playerPos - spawner.transform.position).sqrMagnitude;
            float weight = 1f / (sqrDist + spawnerDistanceBias);

            roll -= weight;
            if (roll <= 0f)
                return spawner;
        }

        return null;
    }

    public T SpawnEnemy<T>(T prefab, Vector3 position)
        where T : BaseEnemy
    {
        T enemy = PoolingManager.Instance.Spawn(
            prefab,
            position,
            Quaternion.identity
        );

        activeEnemies.Add(enemy);

        float difficulty =
            1 + Mathf.Pow((float)timeSinceStartPlaying / 60f, 1.1f);

        enemy.Initialize(difficulty);
        return enemy;
    }

    // -------- RETURN --------
    public void ReturnEnemy(BaseEnemy enemy)
    {
        if (!activeEnemies.Remove(enemy))
            return;

        enemy.ClearEffects();
        PoolingManager.Instance.Despawn(enemy);
    }

    // -------- CLEANUP --------
    public void Initialize()
    {
        foreach (var enemy in activeEnemies)
            PoolingManager.Instance.Despawn(enemy);

        activeEnemies.Clear();
        GameInitializationManager.Instance.enemySystemInitialized = true;
    }

    public void SetTimeSinceStartPlaying(double time)
    {
        timeSinceStartPlaying = time;
    }
}
