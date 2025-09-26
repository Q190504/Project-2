using Unity.Collections;
using UnityEngine;

public class BasePassive : MonoBehaviour
{
    [SerializeField] protected int baseValue;
    protected float value;
    protected int currentLevel;
    private bool initialized;
    public bool IsInitialized() => initialized; 
    [SerializeField] protected float increment;

    [Header("Passive Settings")]
    [SerializeField] protected PassiveType passiveType;
    [SerializeField] protected int level;
    [SerializeField] protected int maxLevel;
    [SerializeField] protected string displayName;
    [SerializeField] protected string description;

    public void Initialize()
    {
        if (!initialized)
        {
            level = 0;
            value = baseValue;
            OnInitialize(); // Call the subclass-specific logic
            initialized = true;
        }
    }

    // Mark as protected virtual so subclasses can override
    protected virtual void OnInitialize() { }

    public virtual void LevelUp()
    {
        currentLevel++;
        if (currentLevel > maxLevel)
            currentLevel = maxLevel;
        else
            OnLevelUp();
    }

    // Mark as protected virtual so subclasses can override
    protected virtual void OnLevelUp() { }

    public float GetValue()
    {
        return value;
    }

    public PassiveType GetPassiveType()
    {
        return passiveType;
    }

    public int GetCurrentLevel()
    { return currentLevel; }

    public int GetMaxLevel()
    { return maxLevel; }

    public string GetDisplayName()
    { return displayName; }

    public string GetDescription()
    { return description; }
}
