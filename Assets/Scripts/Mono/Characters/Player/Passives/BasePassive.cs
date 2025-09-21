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

    protected virtual void LevelUp()
    {
        level++;
        if (level > maxLevel)
            level = maxLevel;
    }

    public float GetValue()
    { 
        return value; 
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }
}
