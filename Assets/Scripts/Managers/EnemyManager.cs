using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Enemy Prefabs")]
    [SerializeField] private RedPig redPigPrefab;
    [SerializeField] private ExplodeSlime explodeSlimePrefab;
    [SerializeField] private ShootingSlime shootingSlimePrefab;
    [SerializeField] private PoisonSlime poisonSlimePrefab;

    private readonly HashSet<BaseEnemy> activeEnemies = new();
    private double timeSinceStartPlaying;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // -------- SPAWN --------
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
