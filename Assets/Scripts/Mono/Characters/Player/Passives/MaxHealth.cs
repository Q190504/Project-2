using UnityEngine;

public class MaxHealth : BasePassive
{
    private PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetMaxHealth(baseValue);
            playerHealth.SetCurrentHealth(baseValue);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void LevelUp()
    {
        base.LevelUp();
        value = value * level;
        int bonusHealth = Mathf.FloorToInt(playerHealth.GetMaxHealth() * value);
        playerHealth.SetMaxHealth(playerHealth.GetMaxHealth() + bonusHealth);
        playerHealth.SetCurrentHealth(playerHealth.GetCurrentHealth() + bonusHealth);
    }
}
