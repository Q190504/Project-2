using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int currentHealth;
    private int maxHealth;

    private Armor armor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (armor == null)
            armor = new Armor();

        UpdateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        // Skip updates if game isn’t playing
        if (!GameManager.Instance.IsPlaying())
            return;
    }

    public void TakeDamage(int amount)
    {
        int finalDamage = Mathf.FloorToInt(Mathf.Max(0, amount - armor.GetValue()));

        currentHealth -= finalDamage;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHPBar();

        if (currentHealth <= 0)
        {
            Debug.Log("Player Died!");
            GameManager.Instance.EndGame(false);
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHPBar();
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

    public void SetMaxHealth(int value)
    {
        maxHealth = value;
    }

    public int GetCurrentHealth()
    { return currentHealth; }


    public int GetMaxHealth()
    { return maxHealth; }
}
