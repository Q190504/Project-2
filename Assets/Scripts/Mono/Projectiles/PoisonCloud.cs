using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    [SerializeField] private List<InGameObjectType> damageTargetObjectTypes;

    float tick;
    float tickTimer;
    float cloudRadius;
    int damagePerTick;
    float existDurationTimer;
    float bonusMoveSpeedPerTargetInTheCloudModifier;
    int totalEnemiesCurrentlyInTheCloud;

    [SerializeField] PoisonCloudPublisherSO onCloudReturn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying()) return;

        existDurationTimer -= Time.deltaTime;
        if (existDurationTimer <= 0)
        {
            onCloudReturn?.RaiseEvent(this);
            ProjectilesManager.Instance.ReturnPoisonCloud(this);
            return;
        }

        tickTimer -= Time.deltaTime;
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

    public int GetTotalEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, cloudRadius / 2);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<ObjectType>(out ObjectType objectType)
                && damageTargetObjectTypes.Contains(objectType.InGameObjectType))
                totalEnemiesCurrentlyInTheCloud++;
        }

        return totalEnemiesCurrentlyInTheCloud;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying()) return;

        // Set canSpawnNewCloud = false to Paw Print Poisoner Weapon
        if (collision.TryGetComponent<ObjectType>(out ObjectType objectType)
            && objectType.InGameObjectType == InGameObjectType.Player)
        {
            if (collision.TryGetComponent<PawPrintPoisonerWeapon>(out PawPrintPoisonerWeapon pawPrintPoisonerWeapon))
                pawPrintPoisonerWeapon.SetCanSpawnNewCloud(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying()) return;

        // Set canSpawnNewCloud = true to Paw Print Poisoner Weapon
        if (collision.TryGetComponent<ObjectType>(out ObjectType objectType)
            && objectType.InGameObjectType == InGameObjectType.Player)
        {
            if (collision.TryGetComponent<PawPrintPoisonerWeapon>(out PawPrintPoisonerWeapon pawPrintPoisonerWeapon))
                pawPrintPoisonerWeapon.SetCanSpawnNewCloud(true);
        }
    }

    public void Initialize(float tick, int damagePerTick, float cloudRadius, float maximumCloudDuration,
    float bonusMoveSpeedPerTargetInTheCloudModifier)
    {
        this.tick = tick;
        this.tickTimer = tick;
        this.cloudRadius = cloudRadius;
        this.damagePerTick = damagePerTick;
        this.existDurationTimer = maximumCloudDuration;
        this.bonusMoveSpeedPerTargetInTheCloudModifier = bonusMoveSpeedPerTargetInTheCloudModifier;
        this.totalEnemiesCurrentlyInTheCloud = 0;
    }

    void DebugDrawSphere(Vector3 center, float radius, Color color)
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
