using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EnemySkill2State : EnemyState
{
    public EnemySkill2State(BaseEnemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {

    }

    public override void AnimationTriggerEvent(BaseEnemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        enemy.EnemySkill2Base.DoAnimationTriggerEventLogic(enemy, triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy.EnemySkill2Base.DoEnterLogic(enemy);
    }

    public override void ExitState()
    {
        base.ExitState();
        enemy.EnemySkill2Base.DoExitLogic(enemy);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemy.EnemySkill2Base.DoFrameUpdateLogic(enemy);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemy.EnemySkill2Base.DoPhysicsLogic(enemy);
    }
}
