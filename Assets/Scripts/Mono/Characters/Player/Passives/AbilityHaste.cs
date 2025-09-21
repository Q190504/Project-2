using UnityEngine;

public class AbilityHaste : BasePassive
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void LevelUp()
    {
        base.LevelUp();
    }

    public int GetCooldownTimeAfterReduction(float baseCooldownTime)
    {
        return Mathf.RoundToInt(baseCooldownTime * (100 / (100 + value)));
    }
}
