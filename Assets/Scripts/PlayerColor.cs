using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerColor : NetworkBehaviour
{
    [Networked, Tooltip("The color of the player's networked object")]
    public Color NetworkedColor { get; set; }

    private ChangeDetector _changes;
    private MeshRenderer meshRendererToChange;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        meshRendererToChange = GetComponentInChildren<MeshRenderer>();

        if (NetworkedColor != null)
        {
            meshRendererToChange.material.color = NetworkedColor;
        }
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(NetworkedColor):
                    meshRendererToChange.material.color = NetworkedColor;
                    break;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {
            if (HasStateAuthority && inputData.colorActionValue)
            {
                Debug.Log("Color Change");

                // Define alpha channel as a constant
                const float AlphaChannel = 1f;

                // Generate random color with full opacity
                var randomColor = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    AlphaChannel
                );

                NetworkedColor = randomColor;
            }
        }
    }
}
