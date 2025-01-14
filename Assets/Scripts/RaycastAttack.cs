using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastAttack : NetworkBehaviour
{
    [SerializeField, Tooltip("Base damage inflicted by the attack")]
    private int damage = 10; // Replaced 'Damage' to align with naming conventions.

    [SerializeField, Tooltip("Distance the raycast can travel")]
    private float shootDistance = 5f;

    [SerializeField] private InputAction attack;
    [SerializeField] private InputAction attackLocation;

    [Networked, Tooltip("Tracks whether the gun buff is active")]
    private bool hasGunBuff { get; set; } = false;

    private bool attackPressed;

    private const int DamageMultiplierWithGunBuff = 4; // Multiplier for damage when gun buff is active

    private void OnEnable()
    {
        attack.Enable();
        attackLocation.Enable();
    }

    private void OnDisable()
    {
        attack.Disable();
        attackLocation.Disable();
    }

    void OnValidate()
    {
        if (attack == null)
            attack = new InputAction(type: InputActionType.Button);
        if (attack.bindings.Count == 0)
            attack.AddBinding("<Mouse>/leftButton");

        if (attackLocation == null)
            attackLocation = new InputAction(type: InputActionType.Value, expectedControlType: "Vector2");
        if (attackLocation.bindings.Count == 0)
            attackLocation.AddBinding("<Mouse>/position");
    }

    void Update()
    {
        if (!HasStateAuthority) return;

        if (attack.WasPerformedThisFrame())
        {
            attackPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (attackPressed)
        {
            PerformRaycast();
            attackPressed = false;
        }
    }

    private void PerformRaycast()
    {
        Vector2 attackLocationInScreenCoordinates = attackLocation.ReadValue<Vector2>();
        var camera = Camera.main;
        Ray ray = camera.ScreenPointToRay(attackLocationInScreenCoordinates);
        ray.origin += camera.transform.forward;

        Debug.DrawRay(ray.origin, ray.direction * shootDistance, Color.red, duration: 1f);

        if (Runner.GetPhysicsScene().Raycast(ray.origin, ray.direction, out var hit, shootDistance))
        {
            GameObject hitObject = hit.transform.gameObject;
            Debug.Log($"Raycast hit: {hitObject.name}, Tag: {hitObject.tag}");

            // Apply damage to the hit object
            if (hitObject.TryGetComponent<Health>(out var health))
            {
                int damageToApply = CalculateDamage();
                health.DealDamageRpc(damageToApply);
                Debug.Log($"Applied damage: {damageToApply}");

                if (hitObject.CompareTag("Player")) // Ensure we're hitting a player
                {
                    if (TryGetComponent<Player>(out var attackerPlayer))
                    {
                        attackerPlayer.AddScore(1); // Increment the attacker's score
                        Debug.Log($"Score added to attacker: {attackerPlayer.name}");
                    }
                }
            }
        }
    }

    public void ActivateGunBuff()
    {
        if (HasStateAuthority)
        {
            hasGunBuff = true;
            Debug.Log("Gun buff activated! Damage will be increased.");
        }
    }

    private int CalculateDamage()
    {
        return hasGunBuff ? damage * DamageMultiplierWithGunBuff : damage;
    }
}
