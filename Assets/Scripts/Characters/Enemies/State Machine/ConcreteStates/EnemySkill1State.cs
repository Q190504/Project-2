using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EnemySkill1State : EnemyState
{
    public override void AnimationTriggerEvent(BaseEnemy enemy, BaseEnemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(enemy, triggerType);
        enemy.EnemySkill1Base.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void EnterState(BaseEnemy enemy)
    {
        base.EnterState(enemy);
        enemy.EnemySkill1Base.DoEnterLogic(enemy);
    }

    public override void ExitState(BaseEnemy enemy)
    {
        base.ExitState(enemy);
        enemy.EnemySkill1Base.DoExitLogic(enemy);
    }

    public override void FrameUpdate(BaseEnemy enemy)
    {
        base.FrameUpdate(enemy);
        enemy.EnemySkill1Base.DoFrameUpdateLogic(enemy);
    }

    public override void PhysicsUpdate(BaseEnemy enemy)
    {
        base.PhysicsUpdate(enemy);
        enemy.EnemySkill1Base.DoPhysicsLogic(enemy);
    }
}
