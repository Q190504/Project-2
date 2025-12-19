using UnityEngine;

public class ExplodeSlime : BaseEnemy
{
    private ExplodeSlimeExplosionLogic explodeLogic;

    private void OnEnable()
    {
        explodeLogic = GetComponent<ExplodeSlimeExplosionLogic>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = baseMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void SetAnimation()
    {
        //switch (enemyStateController.CurrentEnemyState)
        //{
        //    case EnemyState.Idle:
        //        animator.SetFloat("x", movement.GetFlowDirection().x);
        //        animator.SetFloat("y", movement.GetFlowDirection().y);

        //        spriteRenderer.flipX = movement.GetFlowDirection().x < 0;

        //        break;
        //    case EnemyState.Run:
        //        animator.SetFloat("x", movement.GetFlowDirection().x);
        //        animator.SetFloat("y", movement.GetFlowDirection().y);

        //        spriteRenderer.flipX = movement.GetFlowDirection().x < 0;
        //        break;
        //    case EnemyState.Shooting:
        //        animator.SetTrigger("shooting");
        //        break;
        //    default:
        //        return;
        //}
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
