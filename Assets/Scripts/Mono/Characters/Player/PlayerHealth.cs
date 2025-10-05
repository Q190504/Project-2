using UnityEngine;

public class PlayerHealth : BaseHealth
{
    private Armor armor;
    private MaxHealth maxHealthCompoment;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        armor = GetComponent<Armor>();
        maxHealthCompoment = GetComponent<MaxHealth>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Skip updates if game isn’t playing
        if (!GameManager.Instance.IsPlaying())
            return;
    }

    public override void TakeDamage(int amount)
    {
        int finalDamage = Mathf.FloorToInt(Mathf.Max(0, amount - armor.GetValue()));

        currentHealth -= finalDamage;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHPBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHPBar();
    }

    public override void Die()
    {
        animator.SetTrigger("die");
        Debug.Log("Player Died!");
        GameManager.Instance.EndGame(false);
    }

    private void UpdateHPBar()
    {
        GamePlayUIManager.Instance.UpdateHPBar(currentHealth, maxHealth);
    }

    public void SetCurrentHealth(int value)
    {
        currentHealth = value;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHPBar();
    }

    public void Initialize()
    {
        SetMaxHealth((int)maxHealthCompoment.GetValue());
        SetCurrentHealth((int)maxHealthCompoment.GetValue());
        GameInitializationManager.Instance.playerHealthInitialized = true;
    }

    public void SetMaxHealth(int value)
    {
        maxHealth = value;
    }

    public int GetCurrentHealth()
    { return currentHealth; }


    public int GetMaxHealth()
    { return maxHealth; }
}
