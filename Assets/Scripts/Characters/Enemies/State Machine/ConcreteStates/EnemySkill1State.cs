using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EnemySkill1State : EnemyState
{
    public EnemySkill1State(BaseEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {

    }

    public override void AnimationTriggerEvent(BaseEnemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        enemy.EnemySkill1Base.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.EnemySkill1Base.DoEnterLogic(enemy);
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.EnemySkill1Base.DoExitLogic(enemy);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.EnemySkill1Base.DoFrameUpdateLogic(enemy);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemy.EnemySkill1Base.DoPhysicsLogic(enemy);
    }
}
