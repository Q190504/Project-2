using UnityEngine;

public class ExplodeSlimeExplodeVFX : BaseVFX
{
    public void PlaySFX()
    {
        AudioManager.Instance.PlayExplodeSlimeExplosionSFX();
    }
}
