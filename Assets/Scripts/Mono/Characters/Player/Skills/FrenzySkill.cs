using UnityEngine;

public class FrenzySkill : BaseSkill, IEffectListener
{
    public EffectType EffectType => EffectType.Frenzy;

    [Header("Frenzy Settings")]
    [SerializeField] private float frenzyBonusPercent;
    [SerializeField] private float baseDuration;
    [SerializeField] private float maxIncreaseDuration;
    [SerializeField] private float increaseDurationPerLevel;
    [Range(0f, 1f)]
    [SerializeField] private float fireRateReductionPercent;
    [Range(0f, 1f)]
    [SerializeField] private float bonusDamagePercent;
    [Range(0f, 1f)]
    [SerializeField] private float bonusMovementSpeedPercent;
    [Range(1f, 5f)]
    [SerializeField] private float hpCostPerShotPercentv;

    private EffectSource effectSource;

    private PlayerLevel playerLevel;
    private AbilityHaste abilityHaste;
    private EffectManager effectManager;

    private void Start()
    {
        playerLevel = GetComponent<PlayerLevel>();
        abilityHaste = GetComponent<AbilityHaste>();
        effectManager = GetComponent<EffectManager>();

        effectSource = EffectSource.Get("Player");
    }

    protected override void Initialize()
    {
        base.Initialize();

        skillID = PlayerSkillID.Frenzy;
        cooldownTime = 7f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Input.GetKeyDown(KeyCode.E) && !isActive)
            Activate();
    }

    public override bool Activate()
    {
        if (!IsOffCooldown()) return false;

        float duration = Mathf.Min(
            maxIncreaseDuration,
            baseDuration + playerLevel.GetCurrentLevel() * increaseDurationPerLevel
        );

        if (!effectManager.HasEffect(EffectType.Frenzy) && effectSource != null)
            effectManager.ApplyEffect(EffectType.Frenzy, duration, 0, effectSource);

        return true;
    }

    public override void Deactivate()
    {
        isActive = false;
        GamePlayUIManager.Instance.RemoveEffectImage(EffectType.Frenzy);
    }

    public float GetFrenzyBonusPercent()
    {
        return frenzyBonusPercent;
    }

    public float GetCooldownTime()
    {
        return cooldownTime;
    }

    public void OnEffectApplied(BaseEffect effect)
    {
        if (effect.Type == EffectType.Frenzy)
        {
            isActive = true;

            //Add Frenzy Effect Image
            if (GamePlayUIManager.Instance.GetEffectIndexes(EffectType.Frenzy) == -1)
                GamePlayUIManager.Instance.AddEffectImage(EffectType.Frenzy);
        }
    }

    public void OnEffectRefreshed(BaseEffect effect)
    {
        if (effect.Type == EffectType.Frenzy)
        {
            isActive = true;

            //Add Frenzy Effect Image
            if (GamePlayUIManager.Instance.GetEffectIndexes(EffectType.Frenzy) == -1)
                GamePlayUIManager.Instance.AddEffectImage(EffectType.Frenzy);
        }
    }

    public void OnEffectExpired(BaseEffect effect)
    {
        if (effect.Type == EffectType.Frenzy)
        {
            Deactivate();

            float finalCooldownTime = abilityHaste.GetCooldownTimeAfterReduction(cooldownTime);
            StartCooldown(finalCooldownTime);
        }
    }
}
