using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    [SerializeField] int experience;
    float pullForce = 0;
    private Transform target;
    private bool isBeingPulled = false;
    private bool hasBeenCollected = false;

    public void StartBeingPulled(Transform playerTransform, float pullForce)
    {
        target = playerTransform;
        isBeingPulled = true;
        this.pullForce = pullForce;
    }

    private void FixedUpdate()
    {
        if (/*!hasBeenCollected &&*/ isBeingPulled && target != null)
        {
            Vector3 directionToTarget = math.normalize(target.transform.position - transform.position);

            // Move orb toward player
            transform.position += directionToTarget * pullForce * Time.deltaTime;
        }
    }

    public int GetExperience()
    { return experience; }

    public void Collect()
    {
        hasBeenCollected = true;
        isBeingPulled = false;
        pullForce = 0;
        experience = 0;
        target = null;

        //ExperienceOrbManager.Instance.Return(this);
    }
}