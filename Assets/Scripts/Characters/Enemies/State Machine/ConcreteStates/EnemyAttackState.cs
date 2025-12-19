using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(BaseEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {

    }

    public override void AnimationTriggerEvent(BaseEnemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        enemy.EnemyAttackBase.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.EnemyAttackBase.DoEnterLogic(enemy);
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.EnemyAttackBase.DoExitLogic(enemy);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.EnemyAttackBase.DoFrameUpdateLogic(enemy);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemy.EnemyAttackBase.DoPhysicsLogic(enemy);
    }
}
