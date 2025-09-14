using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    None,
    Frenzy,
    Slow,
    Stun,
    Poison,
}

public interface IEffectListener
{
    EffectType EffectType { get; } // which effect this listener cares about
    void OnEffectApplied(BaseEffect effect);
    void OnEffectRefreshed(BaseEffect effect);
    void OnEffectExpired(BaseEffect effect);
}


public class BaseEffect
{
    public EffectType Type { get; protected set; }
    public float Timer { get; protected set; }
    public float Duration { get; protected set; }
    public float Value { get; protected set; }
    public EffectSource Source { get; protected set; }

    private readonly List<IEffectListener> listeners = new();

    public bool IsActive => Timer > 0f;

    public BaseEffect(EffectType type, float duration, float value, EffectSource source)
    {
        Type = type;
        Timer = duration;
        Duration = duration;
        Value = value;
        Source = source;

        NotifyApplied();
    }

    public void UpdateTimer(float deltaTime)
    {
        if (Timer > 0f)
        {
            Timer -= deltaTime;
            GamePlayUIManager.Instance.UpdateEffectDurationUI(Type, Timer, Duration);
        }
    }

    public void Refresh(float duration, float value, EffectSource source)
    {
        Timer = duration;
        Duration = duration;
        Value = Mathf.Max(Value, value);
        Source = source;

        NotifyRefreshed();
    }

    public virtual void Expire()
    {
        Type = EffectType.None;
        Timer = 0;
        Duration = 0;
        Value = 0;
        Source = null;

        NotifyExpired();
    }

    public void AddListener(IEffectListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void RemoveListener(IEffectListener listener)
    {
        listeners.Remove(listener);
    }

    private void NotifyApplied()
    {
        foreach (var l in listeners) l.OnEffectApplied(this);
    }

    private void NotifyRefreshed()
    {
        foreach (var l in listeners) l.OnEffectRefreshed(this);
    }

    private void NotifyExpired()
    {
        foreach (var l in listeners) l.OnEffectExpired(this);
    }
}
