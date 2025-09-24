using UnityEngine;

public class HealthRegen : BasePassive
{
    [Header("Regen Settings")]
    [SerializeField] float regenInterval = 0.5f;

    private float timer;
    private PlayerHealth playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        timer += Time.deltaTime;

        if (timer >= regenInterval)
        {
            timer = 0f;

            if (playerHealth != null &&
                playerHealth.GetCurrentHealth() < playerHealth.GetMaxHealth() &&
                value > 0)
            {
                playerHealth.Heal(Mathf.FloorToInt(value));
            }
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
        value += increment;
    }

    public override void Initialize()
    {
        base.Initialize();
        timer = 0f;
    }
}
