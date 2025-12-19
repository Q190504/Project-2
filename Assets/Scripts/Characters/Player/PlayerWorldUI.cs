using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWorldUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TMP_Text levelText;

    [SerializeField] GameObject player;
    private PlayerHealth playerHealth;
    private PlayerLevel playerLevel;

    private void Start()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerLevel = player.GetComponent<PlayerLevel>();
        }
        else
            Debug.LogError("Cant find player in PlayerWorldUI");
    }

    private void Update()
    {
        if (playerHealth == null || playerLevel == null) return;

        // Update health
        if (healthBarSlider != null)
        {
            healthBarSlider.minValue = 0;
            healthBarSlider.maxValue = playerHealth.GetMaxHealth();
            healthBarSlider.value = playerHealth.GetCurrentHealth();
        }

        // Update level
        if (levelText != null)
        {
            levelText.text = playerLevel.GetCurrentLevel().ToString();
        }
    }
}
