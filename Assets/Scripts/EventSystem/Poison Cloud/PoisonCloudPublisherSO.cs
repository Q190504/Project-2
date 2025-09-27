using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Poison Cloud Pulisher", menuName = "Scriptable Objects/Events/Poison Cloud Publisher")]
public class PoisonCloudPublisherSO : ScriptableObject
{
    public UnityAction<PoisonCloud> OnEventRaised;

    public void RaiseEvent(PoisonCloud obj)
    {
        OnEventRaised?.Invoke(obj);
    }
}
