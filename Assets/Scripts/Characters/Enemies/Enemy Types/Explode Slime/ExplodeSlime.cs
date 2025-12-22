using UnityEngine;

public class ExplodeSlime : BaseEnemy
{
    private ExplodeSlimeExplosionLogic explodeLogic;

    private void OnEnable()
    {
        explodeLogic = GetComponent<ExplodeSlimeExplosionLogic>();
    }

    public override void Initialize(Transform playerPos, float difficultyMultiplier)
    {
        base.Initialize(playerPos, difficultyMultiplier);

        explodeLogic.Initialize();
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
