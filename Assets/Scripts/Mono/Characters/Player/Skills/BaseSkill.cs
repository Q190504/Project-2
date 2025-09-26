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
    [SerializeField] protected PlayerSkillID skillID;

    protected float cooldownTimer;
    protected float cooldownTime;

    protected bool isActive;
    protected bool isCooldownActive; 

    public virtual void Initialize()
    {
        isActive = false;
        isCooldownActive = false;
        cooldownTimer = 0;
        cooldownTime = baseCooldownTime;
    }

    protected virtual void Update()
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

            if (!isCooldownActive)
            {
                isCooldownActive = true;
            }

            // Update UI
            GamePlayUIManager.Instance.SetSkillCooldownUI(skillID, true);
            GamePlayUIManager.Instance.SetSkillImageOpacity(skillID, false);
            GamePlayUIManager.Instance.UpdateSkillCooldownUI(skillID, cooldownTimer, cooldownTime);
        }
        else
        {
            if (isCooldownActive) 
            {
                isCooldownActive = false;
                GamePlayUIManager.Instance.SetSkillCooldownUI(skillID, false);
                GamePlayUIManager.Instance.SetSkillImageOpacity(skillID, !isActive);
            }
        }
    }

    public bool IsOffCooldown()
    {
        return cooldownTimer <= 0;
    }

    public void StartCooldown(float cooldownTime)
    {
        cooldownTimer = this.cooldownTime = cooldownTime;
    }

    public abstract bool Activate();

    public virtual void Deactivate()
    {
        Initialize();
    }
}
