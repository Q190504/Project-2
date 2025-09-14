using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    private readonly Dictionary<(EffectType, EffectSource), BaseEffect> activeEffects = new();
    private readonly Dictionary<EffectType, IEffectListener> listeners = new();

    private void Awake()
    {
        foreach (var listener in GetComponents<IEffectListener>())
        {
            listeners[listener.EffectType] = listener;
        }
    }

    void FixedUpdate()
    {
        List<(EffectType, EffectSource)> expireds = new List<(EffectType, EffectSource)>();

        foreach (var effectEntry in activeEffects)
        {
            var effect = effectEntry.Value;
            effect.UpdateTimer(Time.deltaTime);

            if(effect.Timer <= 0)
            {
                effect.Expire();
                expireds.Add((effect.Type, effect.Source));
            }
        }

        foreach (var expiredEffect in expireds)
        {
            activeEffects.Remove(expiredEffect);
            GamePlayUIManager.Instance.RemoveEffectImage(expiredEffect.Item1);
        }
    }

    public void ApplyEffect(EffectType type, float duration, float value, EffectSource source)
    {
        var key = (type, source);

        if (activeEffects.TryGetValue(key, out var effect))
        {
            effect.Refresh(duration, value, source);
        }
        else
        {
            effect = EffectFactory.CreateEffect(type, duration, value, source);

            if (listeners.TryGetValue(type, out var listener))
                effect.AddListener(listener);

            activeEffects[key] = effect;
        }
    }

    public bool HasEffect(EffectType type)
    {
        foreach (var effect in activeEffects)
        {
            if (effect.Key.Item1 == type && effect.Value.IsActive)
                return true;
        }
        return false;
    }

    public bool HasEffectOfSource(EffectType type, EffectSource source)
    {
        return activeEffects.TryGetValue((type, source), out var effect) && effect.IsActive;
    }


    public float GetRemainingTime(EffectType type, EffectSource source)
    {
        return activeEffects.TryGetValue((type, source), out var effect) ? effect.Timer : 0f;
    }

    public float GetValue(EffectType type, EffectSource source)
    {
        return activeEffects.TryGetValue((type, source), out var effect) ? effect.Value : 0f;
    }
}

public static class EffectFactory
{
    public static BaseEffect CreateEffect(EffectType type, float duration, float value, EffectSource source)
    {
        return type switch
        {
            EffectType.Frenzy => new FrenzyEffect(duration, value, source),
            _ => new BaseEffect(type, duration, value, source)
        };
    }
}

