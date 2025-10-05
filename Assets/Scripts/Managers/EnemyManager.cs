using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _instance;

    [SerializeField] private int redPigPrepare;
    [SerializeField] private int explodeSlimePrepare;
    public List<GameObject> SpawnerList;

    [Header("Refs")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject redPigPrefab;
    [SerializeField] private GameObject explodeSlimePrefab;

    private Queue<GameObject> inactiveRedPigs;
    private List<GameObject> activeRedPigs;
    private Transform redPigPool;
    private int inactiveRedPigsCount = 0;

    private Queue<GameObject> inactiveExplodeSlimes;
    private List<GameObject> activeExplodeSlimes;
    private Transform explodeSlimePool;
    private int inactiveExplodeSlimesCount = 0;

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

        inactiveRedPigs = new Queue<GameObject>();
        activeRedPigs = new List<GameObject>();

        inactiveExplodeSlimes = new Queue<GameObject>();
        activeExplodeSlimes = new List<GameObject>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PreparePools();

        PrepareEnemies();
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

    private void PreparePools()
    {
        redPigPool = new GameObject("RedPigPool").transform;
        redPigPool.SetParent(transform);

        explodeSlimePool = new GameObject("explodeSlimePool").transform;
        explodeSlimePool.SetParent(transform);
    }

    public void SpawnEnemy(Vector3 position)
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        float difficultyMultiplier = 1 + Mathf.Pow((float)timeSinceStartPlaying / 60f, 1.2f);

        GameObject redPig = TakeRedPig();
        RedPig redPigComponent = redPig.GetComponent<RedPig>();
        redPigComponent.Initialize(position, difficultyMultiplier);

        //GameObject exSlime = TakeExplodeSlime();
        //ExplodeSlime explodeSlimeComponent = exSlime.GetComponent<ExplodeSlime>();
        //explodeSlimeComponent.Initialize(position, difficultyMultiplier);
    }

    private void PrepareEnemies()
    {
        PrepareRedPig();
        PrepareExplodeSlime();
    }

    #region Red Pig

    private void PrepareRedPig()
    {
        if (redPigPrefab == null) return;

        for (int i = 0; i < redPigPrepare; i++)
        {
            GameObject pig = Instantiate(redPigPrefab, redPigPool);
            pig.SetActive(false);
            inactiveRedPigs.Enqueue(pig);
            inactiveRedPigsCount++;
        }
    }

    public GameObject TakeRedPig()
    {
        if (inactiveRedPigs.Count <= 0)
            PrepareRedPig();

        GameObject pig = inactiveRedPigs.Dequeue();
        inactiveRedPigsCount--;
        activeRedPigs.Add(pig);

        pig.transform.SetParent(null);
        pig.SetActive(true);

        return pig;
    }

    public void ReturnRedPig(GameObject pig)
    {
        if (pig.TryGetComponent<EffectManager>(out EffectManager effectManager))
            effectManager.ClearAllEffects();

        pig.SetActive(false);
        pig.transform.SetParent(redPigPool);

        activeRedPigs.Remove(pig);
        inactiveRedPigs.Enqueue(pig);
        inactiveRedPigsCount++;
    }

    private void ClearAllRedPig()
    {
        if (activeRedPigs != null && activeRedPigs.Count > 0)
        {
            foreach (var pig in activeRedPigs)
                ReturnRedPig(pig);

            activeRedPigs.Clear();
        }
    }

    public int GetRedPigPrepare()
    {
        return redPigPrepare;
    }

    #endregion


    #region Explode Slime

    private void PrepareExplodeSlime()
    {
        if (explodeSlimePrefab == null) return;

        for (int i = 0; i < explodeSlimePrepare; i++)
        {
            GameObject go = Instantiate(explodeSlimePrefab, explodeSlimePool);
            go.SetActive(false);
            inactiveExplodeSlimes.Enqueue(go);
            inactiveExplodeSlimesCount++;
        }
    }

    public GameObject TakeExplodeSlime()
    {
        if (inactiveExplodeSlimes.Count <= 0)
            PrepareExplodeSlime();

        GameObject go = inactiveExplodeSlimes.Dequeue();
        inactiveExplodeSlimesCount--;

        activeExplodeSlimes.Add(go);

        go.transform.SetParent(null);
        go.SetActive(true);

        return go;
    }

    public void ReturnExplodeSlime(GameObject go)
    {
        if (go.TryGetComponent<EffectManager>(out EffectManager effectManager))
            effectManager.ClearAllEffects();

        go.SetActive(false);
        go.transform.SetParent(explodeSlimePool);

        activeExplodeSlimes.Remove(go);
        inactiveExplodeSlimes.Enqueue(go);
        inactiveExplodeSlimesCount++;
    }

    private void ClearAllExplodeSlime()
    {
        if (activeExplodeSlimes != null && activeExplodeSlimes.Count > 0)
        {
            foreach (GameObject go in activeExplodeSlimes)
                ReturnExplodeSlime(go);

            activeExplodeSlimes.Clear();
        }

    }

    public int GetExplodeSlimePrepare()
    {
        return explodeSlimePrepare;
    }

    #endregion

    public void Initialize()
    {
        waveTimer = initialSpawnDelay;
        individualEnemyDelayTimer = 0;
        waveInterval = baseInterval;
        enemiesPerWave = baseEnemiesPerWave;
        enemiesToSpawnCounter = 0;

        ClearAllEnemies();

        GameInitializationManager.Instance.enemySystemInitialized = true;
    }

    public void ClearAllEnemies()
    {
        ClearAllRedPig();
        ClearAllExplodeSlime();
    }

    public void SetTimeSinceStartPlaying(double time)
    {
        timeSinceStartPlaying = time;
    }
}
