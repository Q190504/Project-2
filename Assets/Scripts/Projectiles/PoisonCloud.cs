using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class PoisonCloud : BaseProjectile
{
    [SerializeField] protected List<InGameObjectType> damageTargetObjectTypes;

    [SerializeField] protected float tick;
    protected float tickTimer;
    [SerializeField] protected float cloudRadius;
    protected int damagePerTick;
    [SerializeField] protected float existDuration;
    protected float existDurationTimer;

    protected virtual void CheckExistTime()
    {
        if (!GameManager.Instance.IsPlaying()) return;

        existDurationTimer -= Time.deltaTime;
        if (existDurationTimer <= 0)
        {
            ReturnCloud();
            return;
        }
        tickTimer -= Time.deltaTime;
    }

    protected abstract void ReturnCloud();

    protected virtual void DealDamge()
    {
        if (tickTimer <= 0)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, cloudRadius / 2);

            //DebugDrawSphere(transform.position, cloudRadius / 2, Color.magenta);

            // Deals damage
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<ObjectType>(out ObjectType objectType)
                    && damageTargetObjectTypes.Contains(objectType.InGameObjectType)
                    && hit.TryGetComponent<IDamageable>(out IDamageable iDamageable))
                {
                    iDamageable.TakeDamage(damagePerTick);
                }
            }

            tickTimer = tick;
        }
    }

    public virtual void Initialize(float tick, int damagePerTick, float cloudRadius, float maximumCloudDuration)
    {
        transform.localScale = new Vector2(cloudRadius, cloudRadius);
        this.tick = tick;
        this.tickTimer = tick;
        this.cloudRadius = cloudRadius;
        this.damagePerTick = damagePerTick;
        this.existDuration = maximumCloudDuration;
        this.existDurationTimer = maximumCloudDuration;
    }

    protected void DebugDrawSphere(Vector3 center, float radius, Color color)
    {
        int segments = 16;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (i / (float)segments) * math.PI * 2;
            float angle2 = ((i + 1) / (float)segments) * math.PI * 2;

            Vector3 p1 = center + new Vector3(math.cos(angle1), math.sin(angle1), 0) * radius;
            Vector3 p2 = center + new Vector3(math.cos(angle2), math.sin(angle2), 0) * radius;

            Debug.DrawLine(p1, p2, color, 0.1f);
        }
    }
}
