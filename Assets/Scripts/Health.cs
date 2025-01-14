using Fusion;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField, Tooltip("UI element to display health")]
    private NumberField healthDisplay;

    [Networked, Tooltip("Initial health of the player")]
    public int NetworkedHealth { get; set; } = 100;

    [Networked, Tooltip("Whether the player has an active shield")]
    public bool HasShield { get; set; } = false;

    private ChangeDetector _changes;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        healthDisplay.SetNumber(NetworkedHealth);
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            if (change == nameof(NetworkedHealth))
            {
                healthDisplay.SetNumber(NetworkedHealth);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(int damage)
    {
        if (HasShield) return; // Ignore damage if shield is active
        NetworkedHealth -= damage;

        if (NetworkedHealth <= 0)
        {
            HandlePlayerDeath();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ActivateShieldRpc()
    {
        HasShield = true;
    }

    private void HandlePlayerDeath()
    {
        if (Object.HasStateAuthority)
        {
            Debug.Log($"{name} has died and will be removed from the game.");
            Runner.Despawn(Object); // Despawn the player's network object
        }
    }
}
