using System.Collections.Generic;
using UnityEngine;

public class ProjectilesManager : MonoBehaviour
{
    private static ProjectilesManager _instance;

    [Header("Prepare")]
    [SerializeField] private int slimeBulletPrepare = 100;
    [SerializeField] private int slimeBeamPrepare = 4;
    [SerializeField] private int poisonCloudPrepare = 24;

    [Header("Prefabs")]
    [SerializeField] private SlimeBullet slimeBulletPrefab;
    [SerializeField] private SlimeBeam slimeBeamPrefab;
    [SerializeField] private PoisonCloud poisonCloudPrefab;

    private Queue<SlimeBullet> inactiveSlimeBullets = new Queue<SlimeBullet>();
    private readonly List<SlimeBullet> slimeBulletsToReclaim = new List<SlimeBullet>();
    private Transform slimeBulletsPool;
    private int slimeBulletCount = 0;

    private Queue<SlimeBeam> inactiveSlimeBeams = new Queue<SlimeBeam>();
    private Transform slimeBeamsPool;
    private int slimeBeamCount = 0;

    private Queue<PoisonCloud> inactivePoisonClouds = new Queue<PoisonCloud>();
    private Transform poisonCloudsPool;
    private int poisonCloudCount = 0;

    public static ProjectilesManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<ProjectilesManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreatePools();

        // Prepare the initial pool of entities
        PreparePoisonCloud();
        PrepareSlimeBeam();
        PrepareSlimeBullet();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreatePools()
    {
        slimeBulletsPool = new GameObject("SlimeBulletsPool").transform;
        slimeBulletsPool.SetParent(transform);

        slimeBeamsPool = new GameObject("SlimeBeamsPool").transform;
        slimeBeamsPool.SetParent(transform);

        poisonCloudsPool = new GameObject("PoisionCloudsPool").transform;
        poisonCloudsPool.SetParent(transform);
    }

    #region Slime Bullet

    private void PrepareSlimeBullet()
    {
        if (slimeBulletPrefab == null) return;

        for (int i = 0; i < slimeBulletPrepare; i++)
        {
            SlimeBullet slimeBulletInstance = Instantiate(slimeBulletPrefab, slimeBulletsPool);
            slimeBulletInstance.gameObject.SetActive(false);
            inactiveSlimeBullets.Enqueue(slimeBulletInstance);
            slimeBulletCount++;
        }
    }

    public SlimeBullet TakeSlimeBullet()
    {
        if (inactiveSlimeBullets.Count <= 0)
            PrepareSlimeBullet();

        SlimeBullet bullet = inactiveSlimeBullets.Dequeue();
        slimeBulletCount--;

        bullet.transform.SetParent(null, false);
        bullet.gameObject.SetActive(true);

        return bullet;
    }

    public void ReturnSlimeBullet(SlimeBullet bullet)
    {
        UnregisterSlimeBulletsToReclaim(bullet);
        bullet.gameObject.SetActive(false);
        bullet.transform.SetParent(slimeBulletsPool.transform, false);
        inactiveSlimeBullets.Enqueue(bullet);
        slimeBulletCount++;
    }

    public void RegisterSlimeBulletsToReclaim(SlimeBullet bullet)
    {
        if (!slimeBulletsToReclaim.Contains(bullet))
            slimeBulletsToReclaim.Add(bullet);
    }

    public void UnregisterSlimeBulletsToReclaim(SlimeBullet bullet)
    {
        if (slimeBulletsToReclaim.Contains(bullet))
            slimeBulletsToReclaim.Remove(bullet);
    }

    public bool HasWaitingSlimeBullet()
    {
        return slimeBulletsToReclaim.Count > 0;
    }

    public List<SlimeBullet> GetActiveBullets()
    {
        return slimeBulletsToReclaim;
    }

    #endregion

    #region SlimeBeam

    private void PrepareSlimeBeam()
    {
        if (slimeBeamPrefab == null) return;

        for (int i = 0; i < slimeBeamPrepare; i++)
        {
            SlimeBeam beam = Instantiate(slimeBeamPrefab, slimeBeamsPool);
            beam.gameObject.SetActive(false);
            slimeBeamCount++;
            inactiveSlimeBeams.Enqueue(beam);
        }
    }

    public SlimeBeam TakeSlimeBeam()
    {
        if (inactiveSlimeBeams.Count <= 0)
            PrepareSlimeBeam();

        SlimeBeam beam = inactiveSlimeBeams.Dequeue();

        beam.transform.SetParent(null, false);
        beam.gameObject.SetActive(true);
        slimeBeamCount--;

        return beam;
    }

    public void ReturnSlimeBeam(SlimeBeam beam)
    {
        if (beam == null) return;

        beam.gameObject.SetActive(false);
        beam.transform.SetParent(slimeBeamsPool, false);
        inactiveSlimeBeams.Enqueue(beam);
        slimeBeamCount++;
    }

    #endregion

    #region Poison Cloud

    private void PreparePoisonCloud()
    {
        if (poisonCloudPrefab == null) return;

        for (int i = 0; i < poisonCloudPrepare; i++)
        {
            PoisonCloud cloud = Instantiate(poisonCloudPrefab, poisonCloudsPool);
            cloud.gameObject.SetActive(false);
            poisonCloudCount++;
            inactivePoisonClouds.Enqueue(cloud);
        }
    }

    public PoisonCloud TakePoisonCloud()
    {
        if (inactivePoisonClouds.Count <= 0)
            PreparePoisonCloud();

        PoisonCloud cloud = inactivePoisonClouds.Dequeue();

        cloud.transform.SetParent(null, false);
        cloud.gameObject.SetActive(true);
        poisonCloudCount--;

        return cloud;
    }

    public void ReturnPoisonCloud(PoisonCloud cloud)
    {
        if (cloud == null) return;

        cloud.gameObject.SetActive(false);
        cloud.transform.SetParent(poisonCloudsPool, false);
        inactivePoisonClouds.Enqueue(cloud);

        poisonCloudCount++;
    }

    #endregion

    public int GetPoisionCloudPrepare()
    {
        return poisonCloudPrepare;
    }

    public int GetSlimeBeamPrepare()
    {
        return slimeBeamPrepare;
    }
}
