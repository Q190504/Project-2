using Unity.Mathematics;
using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    int experience;
    float pullForce = 0;
    private Transform target;
    private bool isBeingPulled = false;

    public void StartBeingPulled(Transform playerTransform, float pullForce)
    {
        target = playerTransform;
        isBeingPulled = true;
        this.pullForce = pullForce;
    }

    private void FixedUpdate()
    {
        if (isBeingPulled && target != null)
        {
            Vector3 directionToTarget = math.normalize(target.transform.position - transform.position);

            // Move orb toward player
            transform.position += pullForce * Time.deltaTime * directionToTarget;
        }
    }

    public int GetExperience()
    { return experience; }

    public void Collect()
    {
        ExperienceOrbManager.Instance.Return(this);
    }

    public void Initialize(int ex)
    {
        isBeingPulled = false;
        pullForce = 0;
        target = null;

        experience = ex;
    }
}