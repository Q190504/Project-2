using System.Collections.Generic;
using UnityEngine;

public class SlimeBulletShooterWeapon : BaseWeapon
{
    [SerializeField] private List<SlimeBulletShooterLevelDataSO> levelDatas;

    private float timer;
    private bool isSlimeFrenzyActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Initialize()
    {
        currentLevel = 1;
        timer = 0;
        isSlimeFrenzyActive = false;
    }
}
