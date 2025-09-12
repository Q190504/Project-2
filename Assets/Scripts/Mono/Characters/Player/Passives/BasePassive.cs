using Unity.Collections;
using UnityEngine;

public class BasePassive : MonoBehaviour
{
    //protected int ID;
    [SerializeField] protected int baseValue;
    protected float value;
    protected int currentLevel;
    [SerializeField] protected float increment;

    [Header("Passive Settings")]
    [SerializeField] protected PassiveType passiveType;
    //[SerializeField] protected int id;
    [SerializeField] protected int level;
    [SerializeField] protected int maxLevel;
    [SerializeField] protected string displayName;
    [SerializeField] protected string description;

    protected virtual void Initialize()
    {
        value = baseValue;
    }

    protected virtual void LevelUp()
    {
        level++;
        if (level > maxLevel)
            level = maxLevel;
    }

    protected virtual void ResetData()
    {
        level = 0;
        value = baseValue;
    }

    public float GetValue()
    { 
        return value; 
    }
}
