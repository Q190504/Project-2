using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected int maxLevel;
    [SerializeField] protected string displayName;
    [SerializeField] protected string description;

    protected int currentLevel;
    protected bool IsActive => currentLevel > 0;

    protected bool isInitialized;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public abstract void Initialize();

    public virtual void LevelUp()
    {
        currentLevel++;
        if (currentLevel > maxLevel)
            currentLevel = maxLevel;
        else
            OnLevelUp();
    }

    protected virtual void OnLevelUp() { }


    public bool IsInitialized()
    {
        return isInitialized;
    }

    public WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public int GetMaxLevel()
    { return maxLevel; }

    public int GetCurrentLevel()
    { return currentLevel; }

    public string GetDisplayName()
    { return displayName; }

    public string GetDescription()
    { return description; }
}
