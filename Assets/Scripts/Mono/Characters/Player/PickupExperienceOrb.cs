using UnityEngine;

public class PickupExperienceOrb : MonoBehaviour
{
    [Header("Pickup Experience Orb Settings")]
    [SerializeField] float pullForce;

    private PlayerLevel playerLevel;
    private PickupRadius pickupRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying())
            return;

        PullOrbs();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ExperienceOrb orb = other.GetComponent<ExperienceOrb>();
        if (orb != null /* && !orb.HasBeenCollected()*/)
        {
            CollectOrb(orb);
        }
    }

    private void PullOrbs()
    {
        // Find all colliders in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius.GetValue() / 2, LayerMask.GetMask("ExperienceOrb"));

        foreach (var hit in hits)
        {
            ExperienceOrb orb = hit.GetComponent<ExperienceOrb>();
            if (orb != null)
            {
                orb.StartBeingPulled(transform, pullForce);
            }
        }

        //DebugDrawSphere(transform.position, value / 2, Color.magenta);
    }

    private void CollectOrb(ExperienceOrb orb)
    {
        // Add XP to player
        playerLevel.AddExperience(orb.GetExperience());

        // Mark orb as collected and return it
        orb.Collect();
    }

    private void DebugDrawSphere(Vector3 center, float radius, Color color)
    {
        int segments = 16;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (i / (float)segments) * Mathf.PI * 2;
            float angle2 = ((i + 1) / (float)segments) * Mathf.PI * 2;

            Vector3 p1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 p2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;

            Debug.DrawLine(p1, p2, color, 0.1f);
        }
    }

    public void Initialize()
    {
        playerLevel = GetComponent<PlayerLevel>();
        pickupRadius = GetComponent<PickupRadius>();
    }
}
