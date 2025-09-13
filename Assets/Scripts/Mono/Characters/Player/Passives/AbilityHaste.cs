using UnityEngine;

public class AbilityHaste : BasePassive
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void LevelUp()
    {
        base.LevelUp();
    }
}
