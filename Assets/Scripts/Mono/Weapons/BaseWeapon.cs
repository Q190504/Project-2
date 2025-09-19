using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] protected WeaponType weaponType;
    //[SerializeField] protected int iD;
    [SerializeField] protected int maxLevel;
    [SerializeField] protected string displayName;
    [SerializeField] protected string description;

    protected int currentLevel;
    protected bool IsActive => currentLevel > 0;

    protected bool hasInitialized;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected abstract void Initialize();

    protected virtual void LevelUp()
    {
        currentLevel++;
        if (currentLevel > maxLevel)
            currentLevel = maxLevel;
    }

    public bool HasInitialized()
    {
        return hasInitialized;
    }
}
