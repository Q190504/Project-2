using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    protected virtual void OnDisable()
    {
        // optional cleanup
    }
}
