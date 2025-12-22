using UnityEngine;

public class EnemyChaseSOBase : ScriptableObject
{
    public virtual void DoEnterLogic(BaseEnemy enemy) 
    {
        enemy.Animator.SetBool("isRun", true);
    }

    public virtual void DoExitLogic(BaseEnemy enemy) 
    {
        ResetValues(enemy);
        enemy.Animator.SetBool("isRun", false);
    }

    public virtual void DoFrameUpdateLogic(BaseEnemy enemy)
    {
        if (GameManager.Instance.IsPlaying() && 
            enemy.CanAttack && enemy.IsWithinStrikingDistance)
        {
            enemy.StateMachine.ChangeState(enemy, enemy.AttackState);
        }
    }
    public virtual void DoPhysicsLogic(BaseEnemy enemy) { }
    public virtual void DoAnimationTriggerEventLogic(BaseEnemy enemy, BaseEnemy.AnimationTriggerType triggerType) { }
    public virtual void ResetValues(BaseEnemy enemy) { }
    public virtual void SetAnimation(BaseEnemy enemy) { }
}
