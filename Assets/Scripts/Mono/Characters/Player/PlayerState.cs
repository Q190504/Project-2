using UnityEngine;

public enum EffectType
{
    Stun,
    Frenzy
}

public class PlayerState : MonoBehaviour
{
    public bool IsStunned {  get; set; }
    public bool IsFrenzyActive { get; set; }
}
