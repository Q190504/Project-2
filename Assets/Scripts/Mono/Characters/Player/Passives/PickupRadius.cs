using UnityEngine;

public class PickupRadius : BasePassive
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void LevelUp()
    {
        base.LevelUp();
        value *= (1 + increment);
    }
}
