using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public override void AnimationTriggerEvent(BaseEnemy enemy, BaseEnemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(enemy, triggerType);
        enemy.EnemyAttackBase.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void EnterState(BaseEnemy enemy)
    {
        base.EnterState(enemy);
        enemy.EnemyAttackBase.DoEnterLogic(enemy);
    }

    public override void ExitState(BaseEnemy enemy)
    {
        base.ExitState(enemy);
        enemy.EnemyAttackBase.DoExitLogic(enemy);
    }

    public override void FrameUpdate(BaseEnemy enemy)
    {
        base.FrameUpdate(enemy);
        enemy.EnemyAttackBase.DoFrameUpdateLogic(enemy);
    }

    public override void PhysicsUpdate(BaseEnemy enemy)
    {
        base.PhysicsUpdate(enemy);
        enemy.EnemyAttackBase.DoPhysicsLogic(enemy);
    }
}
