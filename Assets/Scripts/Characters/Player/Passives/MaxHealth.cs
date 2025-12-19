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

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        value += increment;
        int bonusHealth = Mathf.FloorToInt(playerHealth.GetMaxHealth() * value);
        playerHealth.SetMaxHealth(playerHealth.GetMaxHealth() + bonusHealth);
        playerHealth.Heal(bonusHealth);
    }
}
