using Unity.Mathematics;
using UnityEngine;

public class SlowEffect : BaseEffect
{
    public SlowEffect(EffectType type, float duration, float value, InGameObjectType source, InGameObjectType owner) 
        : base(type, duration, value, source, owner)
    {
    }
}
