using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<BaseWeapon> weapons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weapons = new List<BaseWeapon>();
    }

    public void StartInitialize()
    {
        if (weapons.Count > 0)
            foreach (BaseWeapon weapon in weapons)
                if (weapon.IsInitialized())
                    weapon.Initialize();

        GameInitializationManager.Instance.weaponsInitialized = true;
    }
}
