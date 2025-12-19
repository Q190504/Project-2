using UnityEngine;

public class PoisonSlime : BaseEnemy
{
    private float difficultyMultiplier;

    [Header("Refs")]
    [SerializeField] private EnemyPoisonCloud enemyPoisonCloudPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = baseMaxHealth;
        currentHealth = baseMaxHealth;
    }

    public override void Initialize(float difficultyMultiplier)
    {
        int enemyHP = (int)(BaseMaxHealth + difficultyMultiplier);
        maxHealth = enemyHP;
        currentHealth = enemyHP;

        int enemySpike = (int)(baseSpike + difficultyMultiplier);
        spike = enemySpike;

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
