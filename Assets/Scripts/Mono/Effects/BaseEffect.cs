using UnityEngine;

public class BaseEffect : MonoBehaviour
{
    private float effectDurationTimer;
    private float effectDurationTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ReduceEffectDuration(EffectType effectType)
    {
        effectDurationTimer -= Time.deltaTime;
        // Update effect duration UI
        GamePlayUIManager.Instance.UpdateEffectDurationUI(effectType,
            effectDurationTimer, effectDurationTime);
    }

    public void SetEffectDurationTime(float effectDuration)
    {
        effectDurationTimer = effectDurationTime = effectDuration;
    }

    public float GetEffectDurationTimer()
    {
        return effectDurationTimer;
    }
}
