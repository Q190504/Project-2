using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float smoothTime;

    private MoveSpeed moveSpeed;

    float currentSpeed;
    float targetSpeed;

    private Rigidbody2D rb;
    private EffectManager effectManager;
    private FrenzySkill frenzySkill;

    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        effectManager = GetComponent<EffectManager>();
        frenzySkill = GetComponent<FrenzySkill>();
        moveSpeed = GetComponent<MoveSpeed>();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying()) return;
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsPlaying())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (effectManager.HasEffect(EffectType.Stun))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        targetSpeed = currentSpeed;
        if (effectManager.HasEffect(EffectType.Frenzy))
            targetSpeed += currentSpeed * frenzySkill.GetFrenzyBonusPercent();

        float multiplier = 1f;
        if (effectManager.HasEffect(EffectType.BoostMoveSpeed))
        {
            List<BaseEffect> boostEffects = effectManager.GetEffectOfType(EffectType.BoostMoveSpeed);

            foreach (var effect in boostEffects)
            {
                multiplier *= 1f + math.clamp(effect.Value, 0f, 1f);
            }
        }

        if (effectManager.HasEffect(EffectType.Slow))
        {
            List<BaseEffect> slowEffects = effectManager.GetEffectOfType(EffectType.Slow);

            foreach (var effect in slowEffects)
            {
                multiplier *= 1f - math.clamp(effect.Value, 0f, 1f);
            }
        }

        targetSpeed *= multiplier;

        Vector2 targetVelocity = moveInput * targetSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, smoothTime);
    }

    public float GetCurrentSpeed()
    { return currentSpeed; }

    public float GetSmoothTime()
    { return smoothTime; }

    public void SetCurrentSpeed(float value)
    {
        currentSpeed = value;
    }

    public float GetTargetSpeed()
    { return targetSpeed; }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public void Initialize()
    {
        transform.position = GameManager.Instance.GetPlayerInitialPosition();
        GameInitializationManager.Instance.playerPositionInitialized = true;
        currentSpeed = moveSpeed.GetValue();
        GameInitializationManager.Instance.playerSpeedInitialized = true;
    }
}

