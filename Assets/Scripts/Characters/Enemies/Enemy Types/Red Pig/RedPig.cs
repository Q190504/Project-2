using UnityEngine;

public class RedPig : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = baseMaxHealth;
        currentHealth = baseMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void SetAnimation()
    {
        //switch (enemyStateController.GetState())
        //{
        //    case EnemyState.Idle:
        //        animator.SetFloat("speed", movement.GetSpeed());
        //        break;
        //    case EnemyState.Run:
        //        animator.SetFloat("speed", movement.GetSpeed());
        //        break;
        //    default:
        //        return;
        //}
    }


    public override void Initialize( float difficultyMultiplier)
    {
        int enemyHP = (int)(BaseMaxHealth + difficultyMultiplier);
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
