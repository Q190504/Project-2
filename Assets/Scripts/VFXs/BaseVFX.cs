using UnityEngine;

public abstract class BaseVFX : MonoBehaviour
{
    public virtual void Return()
    {
        VFXManager.Instance.ReturnEffect(this);
    }
}
