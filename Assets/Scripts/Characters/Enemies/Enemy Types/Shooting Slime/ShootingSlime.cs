using UnityEngine;

public class ShootingSlime : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

    public override void Initialize( float difficultyMultiplier)
    {

        int enemyHP = (int)(baseMaxHealth + difficultyMultiplier);
        maxHealth = enemyHP;
        currentHealth = enemyHP;

        int enemySpike = (int)(baseSpike + difficultyMultiplier);
        spike = enemySpike;
    }

    public override void Die()
    {
        EnemyManager.Instance.ReturnEnemy(this);
    }
}
