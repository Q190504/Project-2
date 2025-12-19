using UnityEngine;

public class HitVFX : BaseVFX
{
    public override void Return()
    {
        VFXManager.Instance.ReturnEffect(this);
    }
}
