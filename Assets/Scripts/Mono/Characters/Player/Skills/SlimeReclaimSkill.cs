using System.Collections.Generic;
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

    protected override void Update()
    {
        base.Update();

        HandleSkillReady();
    }

    private float CalculateFinalCooldown()
    {
        float hasteValue = abilityHaste ? abilityHaste.GetValue() : 0f;
        return cooldownTime * (100f / (100f + hasteValue));
    }

    private void HandleSkillReady()
    {
        bool hasWaitingSlimeBullet = ProjectilesManager.Instance.HasWaitingSlimeBullet();

        if (playerHealth.GetCurrentHealth() > 0 && hasWaitingSlimeBullet)
        {
            GamePlayUIManager.Instance.SetSkillImageOpacity(PlayerSkillID.Reclaim, true);

            if (Input.GetKeyDown(KeyCode.R) && Activate())
            {
                Deactivate();
                float finalCooldownTime = CalculateFinalCooldown();
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
        List<SlimeBullet> activeBullets = ProjectilesManager.Instance.GetActiveBullets();
        var bulletsCopy = new List<SlimeBullet>(activeBullets);
        foreach (SlimeBullet bullet in bulletsCopy)
        {
            bullet.Summon(damage);
        }
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

    public float GetBulletSpeedWhenSummoned()
    { 
        return bulletSpeedWhenSummoned; 
    }
}
