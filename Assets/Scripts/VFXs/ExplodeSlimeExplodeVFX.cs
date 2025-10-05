using UnityEngine;

public class ExplodeSlimeExplodeVFX : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX()
    {
        AudioManager.Instance.PlayExplodeSlimeExplosionSFX();
    }

    public void Return()
    {
        AnimationManager.Instance.ReturnExplodeSlimeExplodeEffect(this.gameObject);
    }
}
