using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public override void AnimationTriggerEvent(BaseEnemy enemy, BaseEnemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(enemy, triggerType);
        enemy.EnemyChaseBase.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void EnterState(BaseEnemy enemy)
    {
        base.EnterState(enemy);
        enemy.EnemyChaseBase.DoEnterLogic(enemy);
    }

    public override void ExitState(BaseEnemy enemy)
    {
        base.ExitState(enemy);
        enemy.EnemyChaseBase.DoExitLogic(enemy);
    }

    public override void FrameUpdate(BaseEnemy enemy)
    {
        base.FrameUpdate(enemy);
        enemy.EnemyChaseBase.DoFrameUpdateLogic(enemy);
    }

    public override void PhysicsUpdate(BaseEnemy enemy)
    {
        base.PhysicsUpdate(enemy);
        enemy.EnemyChaseBase.DoPhysicsLogic(enemy);
    }
}
