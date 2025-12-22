using UnityEngine;

public class EnemyState
{
    public virtual void EnterState(BaseEnemy enemy) { }
    public virtual void ExitState(BaseEnemy enemy) { }
    public virtual void FrameUpdate(BaseEnemy enemy) { }
    public virtual void PhysicsUpdate(BaseEnemy enemy) { }
    public virtual void AnimationTriggerEvent(BaseEnemy enemy, BaseEnemy.AnimationTriggerType triggerType) { }

}
