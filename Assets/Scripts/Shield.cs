using Fusion;
using UnityEngine;

public class Shield : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Health>(out var health))
        {
            health.ActivateShieldRpc();
            Runner.Despawn(Object); // Destroy shield after pickup
        }
    }
}
