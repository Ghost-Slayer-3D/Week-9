using Fusion;
using UnityEngine;

public class BallSpawner : NetworkBehaviour
{
    [Networked, Tooltip("Timer for spawning balls")]
    private TickTimer spawnTimer { get; set; }

    [SerializeField, Tooltip("Time interval between ball spawns in seconds")]
    private float timeBetweenSpawns = 1f;

    [SerializeField, Tooltip("Prefab to spawn as a ball")]
    private NetworkObject prefabToSpawn;

    public override void Spawned()
    {
    }

    public override void FixedUpdateNetwork()
    {
        if (spawnTimer.ExpiredOrNotRunning(Runner))
        {
            Runner.Spawn(prefabToSpawn,
                transform.position, transform.rotation,
                Object.InputAuthority);
            spawnTimer = TickTimer.CreateFromSeconds(Runner, timeBetweenSpawns);
        }
    }
}
