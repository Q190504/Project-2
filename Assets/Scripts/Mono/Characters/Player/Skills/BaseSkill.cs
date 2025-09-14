using UnityEngine;

public enum PlayerSkillID
{
    Frenzy,
    Reclaim
}

public abstract class BaseSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    [SerializeField] protected float baseCooldownTime; // Cooldown time at the beginning of the game
    protected PlayerSkillID skillID;

    protected float cooldownTimer;
    protected float cooldownTime;

    protected bool isActive;

    protected virtual void Initialize()
    {
        isActive = false;
        cooldownTimer = 0;
        cooldownTime = baseCooldownTime;
    }

    protected virtual void FixedUpdate()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        HandleCooldown(Time.deltaTime);
    }

    private void HandleCooldown(float deltaTime)
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= deltaTime;

            // Update UI
            GamePlayUIManager.Instance.SetSkillCooldownUI(skillID, true);
            GamePlayUIManager.Instance.SetSkillImageOpacity(skillID, false);
            GamePlayUIManager.Instance.UpdateSkillCooldownUI(skillID, cooldownTimer, cooldownTime);
        }
        else
        {
            GamePlayUIManager.Instance.SetSkillCooldownUI(skillID, false);
            GamePlayUIManager.Instance.SetSkillImageOpacity(skillID, !isActive);
        }
    }

    protected bool IsOffCooldown()
    {
        return cooldownTimer <= 0;
    }

    protected void StartCooldown(float cooldownTime)
    {
        cooldownTimer = this.cooldownTime = cooldownTime;
    }

    public abstract bool Activate();

    public virtual void Deactivate()
    {
        Initialize();
    }
}
