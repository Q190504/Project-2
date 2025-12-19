using UnityEngine;

public interface ITriggerCheckable
{
    bool IsWithinStrikingDistance { get; set; }

    void SetStrikingDistanceBool(bool isWithinStrikingDistance);
}
