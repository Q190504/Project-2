using UnityEngine;

[CreateAssetMenu(fileName = "Idle-Stand Still", menuName = "Scriptable Objects/Enemy/Idle Logic/Stand Still")]
public class EnemyIdleStandStill : EnemyIdleSOBase
{
    public override void DoFrameUpdateLogic(BaseEnemy enemy)
    {
        base.DoFrameUpdateLogic(enemy);

        enemy.MoveEnemy(Vector2.zero);
    }

    public override void DoPhysicsLogic(BaseEnemy enemy)
    {
        base.DoPhysicsLogic(enemy);
    }
}
