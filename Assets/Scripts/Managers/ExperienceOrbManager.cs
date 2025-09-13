using System.Collections.Generic;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;

public class ExperienceOrbManager : MonoBehaviour
{
    private static ExperienceOrbManager _instance;

    [SerializeField] int baseExperiencePerOrb;
    [SerializeField] private int orbPrepare;
    [SerializeField] private float spawnChance; // <= 1  Chance to spawn an orb when an enemy is defeated
    [SerializeField] private ExperienceOrb orbPrefab;

    private Queue<ExperienceOrb> inactiveOrbs;
    private Transform orbsPool;
    private int orbCount = 0;

    [Header("Spawing stats")]
    //private float difficultyMultiplier;
    private double timeSinceStartPlaying;

    public static ExperienceOrbManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<ExperienceOrbManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);

        inactiveOrbs = new Queue<ExperienceOrb>();
        orbsPool = new GameObject("OrbsPool").transform;
        orbsPool.SetParent(transform);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        PrepareOrb();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize()
    {

    }

    private void PrepareOrb()
    {
        if (orbPrefab == null) return;

        for (int i = 0; i < orbPrepare; i++)
        {
            ExperienceOrb orb = GameObject.Instantiate(orbPrefab, orbsPool);
            orb.gameObject.SetActive(false);
            inactiveOrbs.Enqueue(orb);
            orbCount++;
        }
    }

    public void TrySpawnExperienceOrb(Vector3 position)
    {
        if (Random.value < spawnChance)
        {
            SpawnOrb(position);
        }
    }

    public void SpawnOrb(Vector3 position)
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        ExperienceOrb orbInstance = Take();

        orbInstance.transform.position = position;
        float experienceMultiplier = 1 + Mathf.Pow((float)timeSinceStartPlaying / 60f, 1.2f);
        int ex = Mathf.FloorToInt(baseExperiencePerOrb * experienceMultiplier);
        orbInstance.Initialize(ex);
    }

    public ExperienceOrb Take()
    {
        if (inactiveOrbs.Count <= 0)
            PrepareOrb();

        ExperienceOrb orb = inactiveOrbs.Dequeue();
        orbCount--;
        orb.gameObject.SetActive(true);
        return orb;
    }

    public void Return(ExperienceOrb orb)
    {
        orb.Initialize(0);
        orb.gameObject.SetActive(false);
        inactiveOrbs.Enqueue(orb);
        orbCount++;
    }

    public void SetTimeSinceStartPlaying(double time)
    {
        timeSinceStartPlaying = time;
    }
}
