using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Direct To Target", menuName = "Scriptable Objects/Enemy/Chase Logic/Direct To Target")]
public class EnemyChaseDirectToTarget : EnemyChaseSOBase
{
    [SerializeField] private float moveSpeed;
    float targetSpeed;
    float nodeSize;
    int mapWidth;

    // Refs
    Rigidbody2D rb;
    EffectManager effectManager;
    FlowFieldManager flowFieldManager;

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();

        if (flowFieldManager == null)
            flowFieldManager = FlowFieldManager.Instance;
        mapWidth = flowFieldManager.GetMapWidth();
        nodeSize = flowFieldManager.GetNodeSize();
        effectManager = enemy.EffectManager;
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        if (!GameManager.Instance.IsPlaying())
            rb.linearVelocity = Vector2.zero;
        else
        {
            ApplyEffect();
            if (GetMovement().sqrMagnitude < 0.01f)
                rb.linearVelocity = Vector2.zero;
            else
                enemy.MoveEnemy(GetMovement());
        }
    }

    public override void DoPhysicsLogic()
    {
        base.DoPhysicsLogic();
    }

    public override void Initialize(GameObject gameObject, BaseEnemy enemy)
    {
        base.Initialize(gameObject, enemy);
        targetSpeed = moveSpeed;
        rb = enemy.RB;
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }

    public void ApplyEffect()
    {
        targetSpeed = moveSpeed;
        float multiplier = 1f;
        if (effectManager.HasEffect(EffectType.Stun))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (effectManager.HasEffect(EffectType.Slow))
        {
            List<BaseEffect> slowEffects = effectManager.GetEffectOfType(EffectType.Slow);

            foreach (var effect in slowEffects)
            {
                multiplier *= 1f - math.clamp(effect.Value, 0f, 1f);
            }
        }

        targetSpeed *= multiplier;
    }

    public Vector2 GetFlowDirection()
    {
        int x = (int)(enemy.transform.position.x / nodeSize);
        int y = (int)(enemy.transform.position.y / nodeSize);
        int index = x + y * mapWidth;

        return flowFieldManager.GetDirectionFromIndex(index);
    }

    protected Vector2 GetMovement()
    {
        Vector2 flowDirection = GetFlowDirection();
        return new Vector2(flowDirection.x, flowDirection.y) * targetSpeed;
    }
}
