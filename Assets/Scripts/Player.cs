using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    private CharacterController _cc;

    [SerializeField, Tooltip("Base movement speed of the player")]
    private float speed = 5f;

    [SerializeField, Tooltip("Prefab for the ball to spawn when shooting")]
    private GameObject ballPrefab;

    [Networked, Tooltip("Player's current score")]
    public int Score { get; set; } = 0;

    [Networked, Tooltip("Indicates if the player has a speed boost")]
    public bool HasSpeedBoost { get; set; } = false;

    private Camera firstPersonCamera;

    public override void Spawned()
    {
        _cc = GetComponent<CharacterController>();
        if (HasStateAuthority)
        {
            firstPersonCamera = Camera.main;
            var firstPersonCameraComponent = firstPersonCamera.GetComponent<FirstPersonCamera>();
            if (firstPersonCameraComponent && firstPersonCameraComponent.isActiveAndEnabled)
                firstPersonCameraComponent.SetTarget(this.transform);
        }
    }

    private Vector3 moveDirection;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {
            float currentSpeed = HasSpeedBoost ? speed * 2 : speed;
            if (inputData.moveActionValue.magnitude > 0)
            {
                inputData.moveActionValue.Normalize();
                moveDirection = new Vector3(inputData.moveActionValue.x, 0, inputData.moveActionValue.y);
                Vector3 DeltaX = currentSpeed * moveDirection * Runner.DeltaTime;
                _cc.Move(DeltaX);
            }

            if (HasStateAuthority && inputData.shootActionValue)
            {
                Runner.Spawn(ballPrefab, transform.position + moveDirection, Quaternion.LookRotation(moveDirection), Object.InputAuthority);
            }
        }
    }

    public void AddScore(int points)
    {
        Score += points;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(points);
        }
    }
}
