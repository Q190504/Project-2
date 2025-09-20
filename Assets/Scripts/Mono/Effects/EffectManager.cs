using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private GameObject owner;

    private readonly Dictionary<(EffectType, InGameObjectType), BaseEffect> activeEffects = new();
    private readonly Dictionary<EffectType, IEffectListener> listeners = new();

    private void Awake()
    {
        foreach (var listener in GetComponents<IEffectListener>())
        {
            listeners[listener.EffectType] = listener;
        }
    }

    private void OnEnable()
    {
        owner = this.gameObject;
    }

    void FixedUpdate()
    {
        List<(EffectType, InGameObjectType)> expireds = new List<(EffectType, InGameObjectType)>();

        foreach (var effectEntry in activeEffects)
        {
            var effect = effectEntry.Value;
            effect.UpdateTimer(Time.deltaTime);

            if (effect.Timer <= 0)
            {
                effect.Expire();
                expireds.Add((effect.Type, effect.Source));
            }
        }

        foreach (var expiredEffect in expireds)
        {
            activeEffects.Remove(expiredEffect);
            if (owner != null && owner.CompareTag("Player"))
                GamePlayUIManager.Instance.RemoveEffectImage(expiredEffect.Item1);
        }
    }

    public void ApplyEffect(EffectType type, float duration, float value, InGameObjectType source, InGameObjectType owner)
    {
        var key = (type, source);

        if (activeEffects.TryGetValue(key, out var effect))
        {
            effect.Refresh(duration, value);
        }
        else
        {
            effect = EffectFactory.CreateEffect(type, duration, value, source, owner);

            if (listeners.TryGetValue(type, out var listener))
                effect.AddListener(listener);

            activeEffects[key] = effect;
        }
    }

    public void RemoveEffect(EffectType type, InGameObjectType source)
    {
        var key = (type, source);

        if (activeEffects.TryGetValue(key, out var effect))
        {
            effect.Expire();
            activeEffects.Remove(key);
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

    public bool HasEffectOfSource(EffectType type, InGameObjectType source)
    {
        return activeEffects.TryGetValue((type, source), out var effect) && effect.IsActive;
    }

    public List<BaseEffect> GetEffectOfType(EffectType type)
    {
        List<BaseEffect> effects = new List<BaseEffect>();
        foreach (var effect in activeEffects)
        {
            if (effect.Key.Item1 == type && effect.Value.IsActive)
                effects.Add(effect.Value);
        }

        return effects;
    }

    public float GetRemainingTime(EffectType type, InGameObjectType source)
    {
        return activeEffects.TryGetValue((type, source), out var effect) ? effect.Timer : 0f;
    }

    public float GetValue(EffectType type, InGameObjectType source)
    {
        return activeEffects.TryGetValue((type, source), out var effect) ? effect.Value : 0f;
    }
}

public static class EffectFactory
{
    public static BaseEffect CreateEffect(EffectType type, float duration, float value, InGameObjectType source, InGameObjectType owner)
    {
        return type switch
        {
            EffectType.Frenzy => new FrenzyEffect(duration, value, source, owner),
            _ => new BaseEffect(type, duration, value, source, owner)
        };
    }
}

