using Unity.Transforms;
using UnityEngine;

public class CreepHealth : BaseHealth
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Die()
    {
        EnemyManager.Instance.ReturnCreep(this.gameObject);
    }

    public override void TakeDamage(int amount)
    {
        // Create Hit effect
        GameObject hitEffect = AnimationManager.Instance.TakeHitEffect();
        hitEffect.transform.position = transform.position;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            GameManager.Instance.AddEnemyKilled();

            // Try to spawn XP orb
            ExperienceOrbManager.Instance.TrySpawnExperienceOrb(transform.position);

            Die();
        }
    }
}
