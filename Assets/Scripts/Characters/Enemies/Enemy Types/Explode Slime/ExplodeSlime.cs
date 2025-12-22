using UnityEngine;

public class ExplodeSlime : BaseEnemy
{
    private ExplodeSlimeExplosionLogic explodeLogic;

    private void OnEnable()
    {
        explodeLogic = GetComponent<ExplodeSlimeExplosionLogic>();
    }

    public override void Initialize(float difficultyMultiplier)
    {
        int enemyHP = (int)(BaseMaxHealth + difficultyMultiplier);
        currentHealth = enemyHP;

        int enemySpike = (int)(baseSpike + difficultyMultiplier);
        spike = enemySpike;

        explodeLogic.Initialize();

        StateMachine.Initialize(IdleState);
    }

    public override void Die()
    {
        GameManager.Instance.AddEnemyKilled();

        // Try to spawn XP orb
        ExperienceOrbManager.Instance.TrySpawnExperienceOrb(transform.position);

        EnemyManager.Instance.ReturnEnemy(this);
    }

    public override void TakeDamage(int amount)
    {
        // Create Hit effect
        VFXManager.Instance.SpawnVFX(hitEffectPrefab, transform.position, Quaternion.identity, Vector3.one);

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            explodeLogic.Explode();
        }
    }
}
