using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager _instance;

    [SerializeField] private List<BaseWeapon> weapons;

    public static WeaponManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<WeaponManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Update()
    {
        if (GameManager.Instance.GetGameState() == GameState.Initializing &&
            !GameInitializationManager.Instance.weaponsInitialized &&
            weapons.Count > 0)
        {
            foreach (BaseWeapon weapon in weapons)
                if (!weapon.IsInitialized())
                    return;

            GameInitializationManager.Instance.weaponsInitialized = true;
        }
    }

    public void StartInitialize()
    {
        if (weapons.Count > 0)
            foreach (BaseWeapon weapon in weapons)
                if (!weapon.IsInitialized())
                    weapon.Initialize();
    }

    public List<BaseWeapon> GetWeapons()
    {
        return weapons;
    }

    public BaseWeapon GetWeaponWithType(WeaponType type)
    {
        foreach (BaseWeapon weapon in weapons)
        {
            if (weapon.GetWeaponType() == type)
                return weapon;
        }

        return null;
    }
}
