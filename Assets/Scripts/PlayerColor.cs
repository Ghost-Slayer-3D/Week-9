using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerColor : NetworkBehaviour
{
    [Networked]
    public Color NetworkedColor { get; set; }

    private ChangeDetector _changes;
    private MeshRenderer meshRendererToChange;

    public override void Spawned() {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        meshRendererToChange = GetComponentInChildren<MeshRenderer>();
        if (NetworkedColor != null)
            meshRendererToChange.material.color = NetworkedColor;
    }

    public override void Render() {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer)) {
            switch (change) {
                case nameof(NetworkedColor):
                    //var reader = GetPropertyReader<Color>(nameof(NetworkedColor));
                    //var (previous, current) = reader.Read(previousBuffer, currentBuffer);
                    meshRendererToChange.material.color = NetworkedColor;
                    break;
            }
        }
    }

    public override void FixedUpdateNetwork() {
        if (GetInput(out NetworkInputData inputData)) {
            if (HasStateAuthority && inputData.colorActionValue) {
                    Debug.Log("Color Change");
                    var randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                    // Changing the material color here directly does not work since this code is only executed on the client pressing the button and not on every client.
                    // meshRendererToChange.material.color = randomColor;
                    NetworkedColor = randomColor;
            }
        }
    }
}
