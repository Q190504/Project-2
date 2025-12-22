using UnityEngine;

public class EnemyIdleSOBase : ScriptableObject
{
    public virtual void DoEnterLogic(BaseEnemy enemy)
    {
        enemy.Animator.SetBool("isIlde", true);
    }

    public virtual void DoExitLogic(BaseEnemy enemy)
    {
        enemy.Animator.SetBool("isIlde", false);

        ResetValues(enemy);
    }

    public virtual void DoFrameUpdateLogic(BaseEnemy enemy)
    {
        if (GameManager.Instance.IsPlaying())
        {
            enemy.StateMachine.ChangeState(enemy, enemy.ChaseState);
            return;
        }
        else if (enemy.CanAttack && enemy.IsWithinStrikingDistance)
        {
            enemy.StateMachine.ChangeState(enemy, enemy.AttackState);
            return;
        }
    }
    public virtual void DoPhysicsLogic(BaseEnemy enemy) { }
    public virtual void DoAnimationTriggerEventLogic(BaseEnemy enemy, BaseEnemy.AnimationTriggerType triggerType) { }
    public virtual void ResetValues(BaseEnemy enemy) { }
    public virtual void SetAnimation(BaseEnemy enemy) { }
}
