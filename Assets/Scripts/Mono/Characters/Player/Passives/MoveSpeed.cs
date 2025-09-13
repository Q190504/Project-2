using UnityEngine;

public class MoveSpeed : BasePassive
{
    private PlayerMovement playerMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();

        playerMovement = GetComponent<PlayerMovement>();
    }

    protected override void LevelUp()
    {
        base.LevelUp();
        value += increment; 
        float newMoveSpeed = playerMovement.GetCurrentSpeed() * (1 + value);
        playerMovement.SetCurrentSpeed(newMoveSpeed);
    }
}
