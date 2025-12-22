using UnityEngine;

public class RedPig : BaseEnemy
{
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
