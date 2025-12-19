using UnityEngine;

public class ExplodeSlimeExplodeVFX : BaseVFX
{
    public void PlaySFX()
    {
        AudioManager.Instance.PlayExplodeSlimeExplosionSFX();
    }

    public override void Return()
    {
        VFXManager.Instance.ReturnEffect(this);
    }
}
