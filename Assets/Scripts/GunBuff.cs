using Fusion;
using UnityEngine;

public class GunBuff : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Ensure the object entering the trigger has a RaycastAttack component
        other.TryGetComponent<RaycastAttack>(out var raycastAttack);
        // Activate the gun buff for the player
        raycastAttack.ActivateGunBuff();
        // Log for debugging
        Debug.Log($"{other.name} picked up the gun buff!");
        Runner.Despawn(Object);
    }
}
