using UnityEngine;

public class EnemyAttackSOBase : ScriptableObject
{
    public virtual void DoEnterLogic(BaseEnemy enemy) 
    {
        enemy.Animator.SetBool("isAttack", true);
    }

    public virtual void DoExitLogic(BaseEnemy enemy)
    {
        ResetValues(enemy);
        enemy.Animator.SetBool("isAttack", false);
    }

    public virtual void DoFrameUpdateLogic(BaseEnemy enemy) { }
    public virtual void DoPhysicsLogic(BaseEnemy enemy) { }
    public virtual void DoAnimationTriggerEventLogic(BaseEnemy enemy, BaseEnemy.AnimationTriggerType triggerType) { }
    public virtual void ResetValues(BaseEnemy enemy) { }
    public virtual void SetAnimation(BaseEnemy enemy) { }
}
