using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _instance;

    [SerializeField] private int creepPrepare;
    public List<GameObject> SpawnerList;

    [Header("Refs")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject creepPrefab;

    private Queue<GameObject> inactiveCreeps;
    private Transform creepPool;
    private int creepCount = 0;

    Dictionary<GameObject, int> spawnerQueue = new Dictionary<GameObject, int>();

    [Header("Spawning stats")]
    [SerializeField] private int baseEnemiesPerWave;        // Base number of enemies per wave
    private int enemiesPerWave;                             // Number of enemies per wave
    private int enemiesToSpawnCounter;

    [SerializeField] private float initialSpawnDelay;       // Time before the first wave
    [SerializeField] private float minInterval;             // The lowest possible interval to prevent overwhelming the player
    [SerializeField] private float baseInterval;            // The starting interval between spawns at time 0
    private float waveInterval;                             // Time between waves
    private float waveTimer;
    [SerializeField] private float spawnAcceleration;       // Rate at which the interval decreases per minute
    [SerializeField] private float individualEnemyDelay;    // Delay between spawning each enemy in a wave
    private float individualEnemyDelayTimer;

    //private float difficultyMultiplier;
    private double timeSinceStartPlaying;

    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<EnemyManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);

        inactiveCreeps = new Queue<GameObject>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creepPool = new GameObject("CreepPool").transform;
        creepPool.SetParent(transform);

        Initialize();

        PrepareEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        #region Spawn Enemies

        if (waveTimer <= 0)
        {
            if (enemiesToSpawnCounter <= 0 && spawnerQueue.Count == 0)
            {
                // Set time till new wave
                waveInterval = Mathf.Max(minInterval, baseInterval - ((float)timeSinceStartPlaying / 60f) * spawnAcceleration);
                waveTimer = waveInterval;

                // Set enemies per wave
                enemiesPerWave = baseEnemiesPerWave + Mathf.FloorToInt((float)timeSinceStartPlaying / 2f);
                enemiesToSpawnCounter = enemiesPerWave;

                spawnerQueue.Clear();

                // Calculate weights based on distance to player
                Dictionary<GameObject, float> spawnerWeights = new Dictionary<GameObject, float>();
                float totalWeight = 0f;

                foreach (var spawner in SpawnerList)
                {
                    float3 playerPos = player.transform.position;
                    float distance = Vector3.Distance(playerPos, spawner.transform.position);
                    float weight = 1f / (distance + 1f); // Closer = heavier
                    spawnerWeights[spawner] = weight;
                    totalWeight += weight;
                }

                // Distribute enemies to spwaners by weight
                int distributedEnemies = 0;
                foreach (var spawner in SpawnerList)
                {
                    float ratio = spawnerWeights[spawner] / totalWeight;
                    int count = Mathf.FloorToInt(ratio * enemiesPerWave);
                    spawnerQueue.Add(spawner, count);
                    distributedEnemies += count;
                }

                // Distribute remainder enemies
                int remaining = enemiesPerWave - distributedEnemies;
                for (int i = 0; i < remaining; i++)
                {
                    var spawner = SpawnerList[i % SpawnerList.Count];
                    spawnerQueue[spawner]++;
                }
            }

            if (individualEnemyDelayTimer <= 0 && spawnerQueue.Count > 0)
            {
                foreach (var spawner in SpawnerList.ToList())
                {
                    if (!spawnerQueue.ContainsKey(spawner)) continue;

                    int toSpawn = spawnerQueue[spawner];
                    if (toSpawn <= 0)
                    {
                        spawnerQueue.Remove(spawner);
                        continue;
                    }

                    Vector3 spawnPosition = spawner.transform.position;
                    SpawnEnemy(spawnPosition);

                    spawnerQueue[spawner] = toSpawn - 1;

                    if (--enemiesToSpawnCounter <= 0)
                        break;
                }

                individualEnemyDelayTimer = individualEnemyDelay;
            }
            else
            {
                individualEnemyDelayTimer -= Time.deltaTime;
            }
        }
        else
        {
            waveTimer -= Time.deltaTime;
        }

        #endregion
    }

    public void SpawnEnemy(Vector3 position)
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        GameObject creep = TakeCreep();
        Creep creepComponent = creep.GetComponent<Creep>();

        float difficultyMultiplier = 1 + Mathf.Pow((float)timeSinceStartPlaying / 60f, 1.2f);
        creepComponent.Initialize(position, player, difficultyMultiplier);
    }

    private void PrepareEnemy()
    {
        if (creepPrefab == null) return;

        for (int i = 0; i < creepPrepare; i++)
        {
            GameObject creep = Instantiate(creepPrefab, creepPool);
            creep.gameObject.SetActive(false);
            inactiveCreeps.Enqueue(creep);
            creepCount++;
        }
    }

    public GameObject TakeCreep()
    {
        if (inactiveCreeps.Count <= 0)
            PrepareEnemy();

        GameObject creep = inactiveCreeps.Dequeue();
        creepCount--;

        creep.gameObject.SetActive(true);

        return creep;
    }

    public void ReturnCreep(GameObject creep)
    {
        if (creep.TryGetComponent<EffectManager>(out EffectManager effectManager))
            effectManager.ClearAllEffects();

        creep.gameObject.SetActive(false);

        inactiveCreeps.Enqueue(creep);
        creepCount++;
    }

    public void Initialize()
    {
        waveTimer = initialSpawnDelay;
        individualEnemyDelayTimer = 0;
        waveInterval = baseInterval;
        enemiesPerWave = baseEnemiesPerWave;
        enemiesToSpawnCounter = 0;
    }

    public int GetCreepPrepare()
    {
        return creepPrepare;
    }

    public void SetTimeSinceStartPlaying(double time)
    {
        timeSinceStartPlaying = time;
    }
}
