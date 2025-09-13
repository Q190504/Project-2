using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    private int currentLevel;
    [SerializeField] private int maxLevel;
    [SerializeField] private int baseExperienceToNextLevel;
    private int experience;
    private int experienceToNextLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = 1;
        experienceToNextLevel = baseExperienceToNextLevel;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddExperience(int experienceToAdd)
    {
        // Game has ended
        if (!GameManager.Instance.IsPlaying())
            return;

        this.experience += experienceToAdd;
        if (experience >= experienceToNextLevel)
            LevelUp();

        // Update XP Bar
        UpdateXPBar(currentLevel, experience, experienceToNextLevel);
    }

    public void LevelUp()
    {
        currentLevel++;
        if (currentLevel >= maxLevel) // Reach the max level
        {
            currentLevel = maxLevel;
            experience = experienceToNextLevel;
        }
        else
        {
            experience -= experienceToNextLevel;

            // Inscrease the experience needed for the next level
            experienceToNextLevel =
                Mathf.FloorToInt(baseExperienceToNextLevel * Mathf.Pow(1.12f, currentLevel));
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void ResetData()
    {
        currentLevel = 1;
        experienceToNextLevel = baseExperienceToNextLevel;
    }

    public void UpdateXPBar(int currentLevel, int experience, int experienceToNextLevel)
    {
        GamePlayUIManager.Instance.UpdateXPBar(currentLevel, experience, experienceToNextLevel);
    }

    public void Initialized()
    {
        currentLevel = 1;
        experience = 0;
        experienceToNextLevel = baseExperienceToNextLevel;
        UpdateXPBar(currentLevel, experience, experienceToNextLevel);

        // TO DO: Update tracker
    }
}
