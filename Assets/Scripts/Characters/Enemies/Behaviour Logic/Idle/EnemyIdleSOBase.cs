using UnityEngine;

public class EnemyIdleSOBase : ScriptableObject
{
    protected BaseEnemy enemy;
    protected Transform transform;
    protected GameObject gameObject; 
    protected Transform playerTransform;

    public virtual void Initialize(GameObject gameObject, BaseEnemy enemy) 
    { 
        this.gameObject = gameObject; 
        transform = gameObject.transform;
        this.enemy = enemy; 
        playerTransform = GameManager.Instance.GetPlayerGO().transform; 
    }

    public virtual void DoEnterLogic()
    {
        enemy.Animator.SetBool("isIlde", true);
    }

    public virtual void DoExitLogic()
    {
        enemy.Animator.SetBool("isIlde", false);

        ResetValues();
    }

    public virtual void DoFrameUpdateLogic()
    {
        if (GameManager.Instance.IsPlaying())
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return;
        }
        else if (enemy.CanAttack && enemy.IsWithinStrikingDistance)
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
            return;
        }
    }
    public virtual void DoPhysicsLogic() { }
    public virtual void DoAnimationTriggerEventLogic(BaseEnemy.AnimationTriggerType triggerType) { }
    public virtual void ResetValues() { }
    public virtual void SetAnimation() { }
}
