using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RadiantFieldWeapon : BaseWeapon
{
    [Header("Radiant Field Settings")]
    [SerializeField] private List<RadiantFieldLevelDataSO> levelDatas;
    [SerializeField] float timeBetween;
    [SerializeField] private LayerMask targetLayer;

    float timer;
    float radius;
    int damagePerTick;
    float cooldown;
    float slowModifier;
    InGameObjectType inGameObjectType;

    [Header("Refs")]
    [SerializeField] GameObject player;
    private FrenzySkill frenzySkill;
    private GenericDamageModifier genericDamageModifier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Cant found player in RadiantFieldWeapon");
            return;
        }

        frenzySkill = player.GetComponent<FrenzySkill>();
        genericDamageModifier = player.GetComponent<GenericDamageModifier>();

        if (this.gameObject.TryGetComponent<ObjectType>(out ObjectType objectType))
            inGameObjectType = objectType.InGameObjectType;
        else
            inGameObjectType = InGameObjectType.RadiantField;

        isInitialized = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying() || !IsActive)
            return;

        timer -= Time.deltaTime;
        if (timer > 0) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius / 2, targetLayer);

        //DebugDrawSphere(transform.ValueRO.Position, radius / 2, Color.yellow);

        int baseDamage = damagePerTick;
        int finalDamage = (int)(baseDamage * (1 + genericDamageModifier.GetValue() + frenzySkill.GetFrenzyBonusPercent()));

        if (damagePerTick > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IDamageable>(out IDamageable health))
                    health.TakeDamage(finalDamage);
            }
        }

        timer = timeBetween;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GameManager.Instance.IsPlaying() || !IsActive)
            return;

        if (collision.CompareTag("Enemy")
            && collision.TryGetComponent<EffectManager>(out EffectManager effectManager)
            && !effectManager.HasEffectOfSource(EffectType.Slow, inGameObjectType))
        {
            if (collision.TryGetComponent<ObjectType>(out ObjectType objectType))
                effectManager.ApplyEffect(EffectType.Slow, math.INFINITY, slowModifier, inGameObjectType, objectType.InGameObjectType);
            else
            {
                effectManager.ApplyEffect(EffectType.Slow, math.INFINITY, slowModifier, inGameObjectType, InGameObjectType.Unknown);
                Debug.LogWarning("Cant find ObjectType of enemy");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!GameManager.Instance.IsPlaying() || !IsActive)
            return;

        if (collision.CompareTag("Enemy")
            && collision.TryGetComponent<EffectManager>(out EffectManager effectManager)
            && effectManager.HasEffectOfSource(EffectType.Slow, inGameObjectType))
        {
            effectManager.RemoveEffect(EffectType.Slow, inGameObjectType);
        }
    }

    public override void Initialize()
    {
        currentLevel = 0;
        timer = 0;

        damagePerTick = 0;
        cooldown = 0;
        slowModifier = 0;
        radius = 0;
        this.gameObject.transform.localScale = new Vector3(1, 1);

        if (this.gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            spriteRenderer.enabled = false;

        isInitialized = true;
    }

    private RadiantFieldLevelDataSO GetCurrentLevelData()
    {
        return levelDatas[math.min(currentLevel - 1, levelDatas.Count - 1)];
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        RadiantFieldLevelDataSO levelData = GetCurrentLevelData();
        damagePerTick = levelData.damagePerTick;
        cooldown = levelData.cooldown;
        slowModifier = levelData.slowModifier;

        radius = levelData.radius;
        this.gameObject.transform.localScale = new Vector3(radius, radius);

        if (this.gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            spriteRenderer.enabled = true;
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
