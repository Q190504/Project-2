using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float smoothTime;


    float currentSpeed;
    //float boostedSpeed;

    private Rigidbody2D rb;
    private GameManager gameManager;
    private EffectManager effectManager;
    private FrenzySkill frenzySkill;

    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        effectManager = GetComponent<EffectManager>();
        frenzySkill = GetComponent<FrenzySkill>();

        gameManager = GameManager.Instance;
        transform.position = gameManager.GetPlayerInitialPosition();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying()) return;
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        if (!gameManager.IsPlaying())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (effectManager.HasEffect(EffectType.Stun))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float targetSpeed = currentSpeed;
        if (effectManager.HasEffect(EffectType.Frenzy))
            targetSpeed += currentSpeed * frenzySkill.GetFrenzyBonusPercent();

        Vector2 targetVelocity = moveInput * targetSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, smoothTime);
    }

    public float GetCurrentSpeed()
    { return currentSpeed; }

    //public float GetBoostedSpeed()
    //{ return boostedSpeed; }

    public float GetSmoothTime()
    { return smoothTime; }

    public void SetCurrentSpeed(float value)
    {
        currentSpeed = value;
    }
}

