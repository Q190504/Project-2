using UnityEngine;

public class ExplodeSlimeHealth : BaseHealth
{
    private ExplodeSlimeExplosionLogic explodeLogic;

    private void OnEnable()
    {
        explodeLogic = GetComponent<ExplodeSlimeExplosionLogic>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Die()
    {
        GameManager.Instance.AddEnemyKilled();

        // Try to spawn XP orb
        ExperienceOrbManager.Instance.TrySpawnExperienceOrb(transform.position);

        EnemyManager.Instance.ReturnExplodeSlime(this.gameObject);
    }

    public override void TakeDamage(int amount)
    {
        // Create Hit effect
        GameObject hitEffect = AnimationManager.Instance.TakeHitEffect();
        hitEffect.transform.position = transform.position;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            explodeLogic.Explode();
        }
    }
}
