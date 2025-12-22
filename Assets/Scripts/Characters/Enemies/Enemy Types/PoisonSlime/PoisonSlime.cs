using UnityEngine;

public class PoisonSlime : BaseEnemy
{
    private float difficultyMultiplier;

    [Header("Refs")]
    [SerializeField] private EnemyPoisonCloud enemyPoisonCloudPrefab;

    public override void Initialize(Transform playerPos, float difficultyMultiplier)
    {
        base.Initialize(playerPos, difficultyMultiplier);
        this.difficultyMultiplier = difficultyMultiplier;
    }

    public override void Die()
    {
        SpawnPoisonCloud();
        EnemyManager.Instance.ReturnEnemy(this);
    }

    private void SpawnPoisonCloud()
    {
        EnemyPoisonCloud enemyPoisonCloud = ProjectilesManager.Instance.
            Spawn(enemyPoisonCloudPrefab, transform.position, Quaternion.identity);
        enemyPoisonCloud.Initialize(difficultyMultiplier);
    }
}
