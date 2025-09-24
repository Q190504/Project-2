using UnityEngine;

public class MaxHealth : BasePassive
{
    private PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void LevelUp()
    {
        base.LevelUp();
        value += increment;
        int bonusHealth = Mathf.FloorToInt(playerHealth.GetMaxHealth() * value);
        playerHealth.SetMaxHealth(playerHealth.GetMaxHealth() + bonusHealth);
        playerHealth.Heal(bonusHealth);
    }

    public override void Initialize()
    {
        base.Initialize();

        if (playerHealth != null)
        {
            playerHealth.SetMaxHealth(baseValue);
            playerHealth.SetCurrentHealth(baseValue);
        }
    }
}
