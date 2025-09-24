using Unity.Collections;
using UnityEngine;

public class BasePassive : MonoBehaviour
{
    [SerializeField] protected int baseValue;
    protected float value;
    protected int currentLevel;
    protected bool isInitialized;
    [SerializeField] protected float increment;

    [Header("Passive Settings")]
    [SerializeField] protected PassiveType passiveType;
    [SerializeField] protected int level;
    [SerializeField] protected int maxLevel;
    [SerializeField] protected string displayName;
    [SerializeField] protected string description;

    public virtual void Initialize()
    {
        level = 0;
        value = baseValue;

        isInitialized = true;
    }

    public virtual void LevelUp()
    {
        level++;
        if (level > maxLevel)
            level = maxLevel;
    }

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

    public bool IsInitialized()
    {
        return isInitialized;
    }
}
