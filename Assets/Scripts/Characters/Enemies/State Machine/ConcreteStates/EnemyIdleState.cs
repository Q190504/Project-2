using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(BaseEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(BaseEnemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        enemy.EnemyIdleBase.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.EnemyIdleBase.DoEnterLogic(enemy);
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.EnemyIdleBase.DoExitLogic(enemy);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.EnemyIdleBase.DoFrameUpdateLogic(enemy);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemy.EnemyIdleBase.DoPhysicsLogic(enemy);
    }
}
