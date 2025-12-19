using System.Collections.Generic;
using UnityEngine;

public class ExperienceOrbManager : MonoBehaviour
{
    public static ExperienceOrbManager Instance;

    [SerializeField] private ExperienceOrb orbPrefab;
    [SerializeField] private int baseExperiencePerOrb;
    [SerializeField, Range(0f, 1f)] private float spawnChance;

    private readonly HashSet<ExperienceOrb> activeOrbs = new();
    private double timeSinceStartPlaying;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TrySpawnExperienceOrb(Vector3 position)
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        if (Random.value <= spawnChance)
            SpawnOrb(position);
    }

    private void SpawnOrb(Vector3 position)
    {
        ExperienceOrb orb = PoolingManager.Instance.Spawn(
            orbPrefab,
            position,
            Quaternion.identity
        );

        activeOrbs.Add(orb);

        float multiplier =
            1 + Mathf.Pow((float)timeSinceStartPlaying / 60f, 1.2f);

        orb.Initialize(Mathf.FloorToInt(baseExperiencePerOrb * multiplier));
    }

    public void ReturnOrb(ExperienceOrb orb)
    {
        if (!activeOrbs.Remove(orb))
            return;

        PoolingManager.Instance.Despawn(orb);
    }

    public void Initialize()
    {
        foreach (var orb in activeOrbs)
            PoolingManager.Instance.Despawn(orb);

        activeOrbs.Clear();
        GameInitializationManager.Instance.cleanedOrbs = true;
    }

    public void SetTimeSinceStartPlaying(double time)
    {
        timeSinceStartPlaying = time;
    }
}
