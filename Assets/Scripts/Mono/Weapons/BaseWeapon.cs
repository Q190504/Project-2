using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] protected WeaponType weaponType;
    //[SerializeField] protected int iD;
    protected int currentLevel;
    [SerializeField] protected int maxLevel;
    [SerializeField] protected string displayName;
    [SerializeField] protected string description;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected abstract void Initialize();
}
