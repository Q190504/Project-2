using UnityEngine;

public class FrenzySkill : BaseSkill
{
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

    [Header("References")]
    private PlayerState playerState;
    private PlayerLevel playerLevel;
    private AbilityHaste abilityHaste;

    BaseEffect frenzyEffect;

    private void Start()
    {
        playerLevel = GetComponent<PlayerLevel>();
        playerLevel = GetComponentInChildren<PlayerLevel>();
        abilityHaste = GetComponentInChildren<AbilityHaste>();
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

        if (isActive && frenzyEffect != null)
        {
            frenzyEffect.ReduceEffectDuration(EffectType.Frenzy);

            if (frenzyEffect.GetEffectDurationTimer() <= 0)
            {
                // Frenzy ended
                Deactivate();
                float finalCooldownTime = abilityHaste.GetCooldownTimeAfterReduction(cooldownTime);
                StartCooldown(finalCooldownTime);
            }
        }
        else if (frenzyEffect == null)
        {
            Debug.LogError("frenzyEffect is not created");
            return;
        }

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

        if (frenzyEffect == null)
            frenzyEffect = gameObject.AddComponent<BaseEffect>();

        frenzyEffect.SetEffectDurationTime(duration);
        isActive = true;
        playerState.IsFrenzyActive = true;

        //Add Frenzy Effect Image
        if (GamePlayUIManager.Instance.GetEffectIndexes(EffectType.Frenzy) == -1)
            GamePlayUIManager.Instance.AddEffectImage(EffectType.Frenzy);

        return true;
    }

    public override void Deactivate()
    {
        isActive = false;
        playerState.IsFrenzyActive = false;
        GamePlayUIManager.Instance.RemoveEffectImage(EffectType.Frenzy);

        frenzyEffect?.SetEffectDurationTime(0);
    }

    public float GetFrenzyBonusPercent()
    {
        return frenzyBonusPercent;
    }
}
