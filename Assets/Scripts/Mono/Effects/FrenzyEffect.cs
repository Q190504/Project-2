using UnityEngine;

public class FrenzyEffect : BaseEffect
{
    public FrenzyEffect(float duration, float value, InGameObjectType source, InGameObjectType owner)
        : base(EffectType.Frenzy, duration, value, source, owner) { }
}
