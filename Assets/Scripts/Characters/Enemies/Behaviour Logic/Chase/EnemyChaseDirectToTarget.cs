using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Direct To Target", menuName = "Scriptable Objects/Enemy/Chase Logic/Direct To Target")]
public class EnemyChaseDirectToTarget : EnemyChaseSOBase
{
    [SerializeField] private float moveSpeed;

    public override void DoEnterLogic(BaseEnemy enemy)
    {
        base.DoEnterLogic(enemy);

        if (enemy.FlowFieldManager == null)
            enemy.FlowFieldManager = FlowFieldManager.Instance;
    }

    public override void DoExitLogic(BaseEnemy enemy)
    {
        base.DoExitLogic(enemy);
    }

    public override void DoFrameUpdateLogic(BaseEnemy enemy)
    {
        base.DoFrameUpdateLogic(enemy);
        if (!GameManager.Instance.IsPlaying())
            enemy.RB.linearVelocity = Vector2.zero;
        else
        {
            enemy.mapWidth = enemy.FlowFieldManager.GetMapWidth();
            enemy.nodeSize = enemy.FlowFieldManager.GetNodeSize();

            if (GetMovement(enemy).sqrMagnitude < 0.01f)
                enemy.RB.linearVelocity = Vector2.zero;
            else
                enemy.MoveEnemy(GetMovement(enemy));
        }
    }

    public float ApplyEffectToMoveSpeed(BaseEnemy enemy)
    {
        float targetSpeed = moveSpeed;
        float multiplier = 1f;
        if (enemy.EffectManager.HasEffect(EffectType.Stun))
        {
            enemy.RB.linearVelocity = Vector2.zero;
            return 0;
        }

        if (enemy.EffectManager.HasEffect(EffectType.Slow))
        {
            List<BaseEffect> slowEffects = enemy.EffectManager.GetEffectOfType(EffectType.Slow);

            foreach (var effect in slowEffects)
            {
                multiplier *= 1f - math.clamp(effect.Value, 0f, 1f);
            }
        }

        return targetSpeed *= multiplier;
    }

    public Vector2 GetFlowDirection(BaseEnemy enemy)
    {
        int x = (int)(enemy.gameObject.transform.position.x / enemy.nodeSize);
        int y = (int)(enemy.gameObject.transform.position.y / enemy.nodeSize);
        int index = x + y * enemy.mapWidth;

        return enemy.FlowFieldManager.GetDirectionFromIndex(index);
    }

    protected Vector2 GetMovement(BaseEnemy enemy)
    {
        Vector2 flowDirection = GetFlowDirection(enemy);
        return new Vector2(flowDirection.x, flowDirection.y) * ApplyEffectToMoveSpeed(enemy);
    }
}
