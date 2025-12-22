using UnityEngine;

public class ShootingSlime : BaseEnemy
{
    public override void Die()
    {
        EnemyManager.Instance.ReturnEnemy(this);
    }
}
