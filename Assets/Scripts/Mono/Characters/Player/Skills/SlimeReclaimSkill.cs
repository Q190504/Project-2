using UnityEngine;

public class SlimeReclaimSkill : BaseSkill
{
    [Header("Skill Settings")]
    [SerializeField] private int baseDamagePerBullet;
    [SerializeField] private int increaseDamagePerLevel;
    [SerializeField] private float hpHealPercentPerBullet;
    [SerializeField] private float bulletSpeedWhenSummoned;

    private PlayerHealth playerHealth;
    private PlayerLevel playerLevel;
    private AbilityHaste abilityHaste;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerLevel = GetComponent<PlayerLevel>();
        abilityHaste = GetComponent<AbilityHaste>();
    }

    protected override void FixedUpdate()
    {
        if (!GameManager.Instance.IsPlaying()) return;

        float finalCooldownTime = CalculateFinalCooldown();

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            UpdateCooldownUI(finalCooldownTime);
            return;
        }

        HandleSkillReady(finalCooldownTime);
    }

    private float CalculateFinalCooldown()
    {
        float hasteValue = abilityHaste ? abilityHaste.GetValue() : 0f;
        return cooldownTime * (100f / (100f + hasteValue));
    }

    private void UpdateCooldownUI(float finalCooldownTime)
    {
        GamePlayUIManager.Instance.SetSkillCooldownUI(PlayerSkillID.Reclaim, true);
        GamePlayUIManager.Instance.SetSkillImageOpacity(PlayerSkillID.Reclaim, false);
        GamePlayUIManager.Instance.UpdateSkillCooldownUI(PlayerSkillID.Reclaim, cooldownTimer, finalCooldownTime);
    }

    private void HandleSkillReady(float finalCooldownTime)
    {
        bool hasWaitingSlimeBullet = true;
        // bool hasWaitingSlimeBullet = SlimeBulletManager.Instance.HasWaitingSlimeBullet();

        if (playerHealth.GetCurrentHealth() > 0 && hasWaitingSlimeBullet)
        {
            GamePlayUIManager.Instance.SetSkillImageOpacity(PlayerSkillID.Reclaim, true);

            if (Input.GetKeyDown(KeyCode.R) && Activate())
            {
                Deactivate();
                cooldownTimer = finalCooldownTime;
            }
        }
        else
        {
            GamePlayUIManager.Instance.SetSkillImageOpacity(PlayerSkillID.Reclaim, false);
        }
    }

    private void RecallBullets()
    {
        int damage = baseDamagePerBullet + increaseDamagePerLevel * playerLevel.GetCurrentLevel();
        // SlimeBulletManager.Instance.RecallBullets(damage, hpHealPercentPerBullet);
    }

    public override bool Activate()
    {
        if (!IsOffCooldown() || isActive) return false;

        isActive = true;
        RecallBullets();
        return true;
    }

    public override void Deactivate()
    {
        isActive = false;
    }
}
