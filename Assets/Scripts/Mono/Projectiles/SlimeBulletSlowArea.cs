using Unity.Mathematics;
using UnityEngine;

public class SlimeBulletSlowArea : MonoBehaviour
{
    private float slowModifier;
    private float slowRadius;

    private InGameObjectType inGameObjectType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inGameObjectType = this.GetComponent<ObjectType>().InGameObjectType;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        // Cache effect manager (avoid repeated GetComponent calls by using TryGetComponent)
        if (!collision.TryGetComponent(out EffectManager enemyEffectManager))
            return;

        // Ingore if the source matches
        if (enemyEffectManager.HasEffectOfSource(EffectType.Slow, inGameObjectType))
            return;

        float3 toCenter = transform.position - collision.transform.position;
        float distSq = math.lengthsq(toCenter);

        if (distSq > slowRadius * slowRadius || distSq < 0.0001f)
            return;

        // Apply slow effect
        if(collision.TryGetComponent<ObjectType>(out ObjectType objectType))
            enemyEffectManager.ApplyEffect(EffectType.Slow, float.MaxValue, 0, inGameObjectType, objectType.InGameObjectType);
        else
        {
            enemyEffectManager.ApplyEffect(EffectType.Slow, float.MaxValue, 0, inGameObjectType, InGameObjectType.Unknown);
            Debug.LogWarning("Cant find ObjectType of enemy");
        }
        // Apply velocity slow if Rigidbody2D exists
        if (collision.TryGetComponent(out Rigidbody2D enemyRb) && math.lengthsq(enemyRb.linearVelocity) > 0)
        {
            enemyRb.linearVelocity *= (1 - slowModifier);
        }
    }

    public void Initialize(float slowRadius, float slowModifier)
    {
        this.slowRadius = slowRadius;
        this.slowModifier = slowModifier;

        transform.localScale = new Vector3(slowRadius, slowRadius, 0);
        gameObject.SetActive(slowRadius > 0);
    }
}
