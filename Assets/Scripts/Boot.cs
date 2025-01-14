using Fusion;
using UnityEngine;

public class Boot : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            player.HasSpeedBoost = true;
            Runner.Despawn(Object); // Destroy boot after pickup
        }
    }
}
