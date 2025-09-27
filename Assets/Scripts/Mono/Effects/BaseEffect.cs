using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum EffectType
{
    None,
    Frenzy,
    Slow,
    BoostMoveSpeed,
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
    public InGameObjectType Source { get; protected set; }

    protected InGameObjectType owner;

    private readonly List<IEffectListener> listeners = new();

    public bool IsActive => Timer > 0f;

    public BaseEffect(EffectType type, float duration, float value, InGameObjectType source, InGameObjectType owner)
    {
        Type = type;
        Timer = duration;
        Duration = duration;
        Value = value;
        Source = source;
        this.owner = owner;

        NotifyApplied();
    }

    public void UpdateTimer(float deltaTime)
    {
        if (Timer > 0f && Timer != math.INFINITY)
        {
            Timer -= deltaTime;
            if (owner == InGameObjectType.Player)
                GamePlayUIManager.Instance.UpdateEffectDurationUI(Type, Timer, Duration);
        }
    }

    public void Refresh(float duration, float value)
    {
        Timer = duration;
        Duration = duration;
        Value = Mathf.Max(Value, value);

        NotifyRefreshed();
    }

    public virtual void Expire()
    {
        NotifyExpired();

        Type = EffectType.None;
        Timer = 0;
        Duration = 0;
        Value = 0;
        Source = InGameObjectType.Unknown;

        listeners.Clear();
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

    public void NotifyApplied()
    {
        foreach (var l in listeners) l.OnEffectApplied(this);
    }

    public void NotifyRefreshed()
    {
        foreach (var l in listeners) l.OnEffectRefreshed(this);
    }

    void NotifyExpired()
    {
        foreach (var l in listeners) l.OnEffectExpired(this);
    }
}
