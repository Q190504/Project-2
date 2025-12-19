using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(BaseEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {

    }

    public override void AnimationTriggerEvent(BaseEnemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        enemy.EnemyChaseBase.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.EnemyChaseBase.DoEnterLogic(enemy);
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.EnemyChaseBase.DoExitLogic(enemy);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.EnemyChaseBase.DoFrameUpdateLogic(enemy);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemy.EnemyChaseBase.DoPhysicsLogic(enemy);
    }
}
