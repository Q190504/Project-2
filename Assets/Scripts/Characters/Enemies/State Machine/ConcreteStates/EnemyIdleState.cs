using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public override void AnimationTriggerEvent(BaseEnemy enemy, BaseEnemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(enemy, triggerType);
        enemy.EnemyIdleBase.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void EnterState(BaseEnemy enemy)
    {
        base.EnterState(enemy);
        enemy.EnemyIdleBase.DoEnterLogic(enemy);
    }

    public override void ExitState(BaseEnemy enemy)
    {
        base.ExitState(enemy);
        enemy.EnemyIdleBase.DoExitLogic(enemy);
    }

    public override void FrameUpdate(BaseEnemy enemy)
    {
        base.FrameUpdate(enemy);
        enemy.EnemyIdleBase.DoFrameUpdateLogic(enemy);
    }

    public override void PhysicsUpdate(BaseEnemy enemy)
    {
        base.PhysicsUpdate(enemy);
        enemy.EnemyIdleBase.DoPhysicsLogic(enemy);
    }
}
