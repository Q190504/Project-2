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

    private float frenzyTimer;

    [Header("References")]
    private PlayerHealth playerHealth;
    private PlayerLevel playerLevel;

    private void Start()
    {
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
        if (playerLevel == null) playerLevel = GetComponent<PlayerLevel>();
    }

    protected override void Initialize()
    {
        base.Initialize();

        skillID = PlayerSkillID.Frenzy;
        cooldownTime = 7f;
        // set BaseDuration 
    }

    protected override void Update()
    {
        base.Update();

        if (isActive)
        {
            frenzyTimer -= Time.deltaTime;
            if (frenzyTimer <= 0)
            {
                // Frenzy ended
                isActive = false;
                StartCooldown(cooldownTime);
            }
        }

        // Try activating every frame
        TryActivate();
    }

    public override void TryActivate()
    {
        if (!IsOffCooldown()) return;
        if (isActive) return;
        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (!CheckPlayerHealth(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth())) return;

        float duration = Mathf.Min(
            maxIncreaseDuration,
            baseDuration + playerLevel.GetCurrentLevel() * increaseDurationPerLevel
        );

        frenzyTimer = duration;
        isActive = true;
    }

    private bool CheckPlayerHealth(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0) return false;
        return true;
    }

    public float GetFrenzyBonusPercent()
    {
        return frenzyBonusPercent;
    }
}
