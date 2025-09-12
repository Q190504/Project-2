using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] float smoothTime;

    float currentSpeed;
    //float boostedSpeed;

    private Rigidbody2D rb;
    private GameManager gameManager;
    private PlayerState state;
    private FrenzySkill frenzySkill;

    private Vector2 moveInput;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (state == null)
            state = GetComponent<PlayerState>();

        if(frenzySkill == null)
            frenzySkill = GetComponent<FrenzySkill>();

        if (gameManager == null)
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

        if (state.isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float targetSpeed = currentSpeed;
        if (state.frenzyActive)
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

