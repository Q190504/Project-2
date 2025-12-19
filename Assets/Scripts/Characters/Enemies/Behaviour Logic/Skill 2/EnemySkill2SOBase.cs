using UnityEngine;

public class EnemySkill2SOBase : ScriptableObject
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
        enemy.Animator.SetBool("isSkill2", true);
    }

    public virtual void DoExitLogic()
    {
        ResetValues();
        enemy.Animator.SetBool("isSkill2", false);
    }

    public virtual void DoFrameUpdateLogic() { }
    public virtual void DoPhysicsLogic() { }
    public virtual void DoAnimationTriggerEventLogic(BaseEnemy.AnimationTriggerType triggerType) { }
    public virtual void ResetValues() { }
    public virtual void SetAnimation() { }
}
