using UnityEngine;

public class Armor : BasePassive
{
    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        value += increment;
    }   
}
