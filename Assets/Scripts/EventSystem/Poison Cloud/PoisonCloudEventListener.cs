using UnityEngine;
using UnityEngine.Events;

public class PoisonCloudEventListener : MonoBehaviour
{
    [SerializeField] private UnityEvent<PoisonCloud> EventResponse;
    [SerializeField] private PoisonCloudPublisherSO publisher;

    private void OnEnable()
    {
        publisher.OnEventRaised += Respond;
    }

    private void OnDisable()
    {
        publisher.OnEventRaised -= Respond;
    }

    private void Respond(PoisonCloud obj)
    {
        EventResponse?.Invoke(obj);
    }
}
