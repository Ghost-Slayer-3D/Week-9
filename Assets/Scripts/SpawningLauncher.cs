using UnityEngine;
using Fusion;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// This class launches Fusion NetworkRunner and also spawns a new avatar whenever a player joins.
public class SpawningLauncher : EmptyLauncher
{
    [SerializeField] NetworkPrefabRef _playerPrefab;
    [SerializeField] NetworkPrefabRef _shieldPrefab; // Added shield prefab
    [SerializeField] NetworkPrefabRef _bootPrefab;   // Added boot prefab
    [SerializeField] NetworkPrefabRef _gunBuffPrefab;
    [SerializeField] Transform gunBuffSpawnPoint;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform shieldSpawnPoint;    // Added spawn point for shield
    [SerializeField] Transform bootSpawnPoint;      // Added spawn point for boot

    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public override void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} joined");

        bool isAllowedToSpawn = (runner.GameMode == GameMode.Shared)
            ? (player == runner.LocalPlayer) // In Shared mode, the local player is allowed to spawn.
            : runner.IsServer;              // In Host or Server mode, only the server is allowed to spawn.

        if (isAllowedToSpawn)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = spawnPoints[player.AsIndex % spawnPoints.Length].position;
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, /*input authority:*/ player);

            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);

            // Spawn power-ups when the first player joins (server-side only)
            if (runner.IsServer && _spawnedCharacters.Count >= 0)
            {
                Debug.Log("First player joined, spawning power-ups...");
                SpawnPowerUps(runner);
            }
        }
    }

    public override void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player} left");

        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    private void SpawnPowerUps(NetworkRunner runner)
    {
        Debug.Log("Attempting to spawn power-ups...");

        if (_shieldPrefab == null || shieldSpawnPoint == null)
        {
            Debug.LogError("Shield prefab or spawn point is not assigned!");
            return;
        }

        if (_bootPrefab == null || bootSpawnPoint == null)
        {
            Debug.LogError("Boot prefab or spawn point is not assigned!");
            return;
        }

        if (gunBuffSpawnPoint == null || _gunBuffPrefab == null)
        {
            Debug.LogError("Gun buff spawn point or prefab is not assigned!");
            return;
        }

        // Spawn the shield at the specified spawn point
        var shield = runner.Spawn(_shieldPrefab, shieldSpawnPoint.position, Quaternion.identity);
        Debug.Log("Shield spawned at: " + shieldSpawnPoint.position);

        // Spawn the boot at the specified spawn point
        var boot = runner.Spawn(_bootPrefab, bootSpawnPoint.position, Quaternion.identity);
        Debug.Log("Boot spawned at: " + bootSpawnPoint.position);

        var gunBuff = runner.Spawn(_gunBuffPrefab, gunBuffSpawnPoint.position, Quaternion.identity);
        Debug.Log("Gun buff spawned at: " + gunBuffSpawnPoint.position);

        // Ensure correct positions are synced across clients
        if (shield != null)
            shield.transform.position = shieldSpawnPoint.position;

        if (boot != null)
            boot.transform.position = bootSpawnPoint.position;

        if (gunBuff != null)
            gunBuff.transform.position = gunBuffSpawnPoint.position;
    }

    [SerializeField] InputAction moveAction = new InputAction(type: InputActionType.Button);
    [SerializeField] InputAction shootAction = new InputAction(type: InputActionType.Button);
    [SerializeField] InputAction colorAction = new InputAction(type: InputActionType.Button);

    private void OnEnable()
    {
        moveAction.Enable();
        shootAction.Enable();
        colorAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        shootAction.Disable();
        colorAction.Disable();
    }

    void OnValidate()
    {
        // Provide default bindings for the input actions. Based on answer by DMGregory: https://gamedev.stackexchange.com/a/205345/18261
        if (moveAction.bindings.Count == 0)
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
        if (shootAction.bindings.Count == 0)
            shootAction.AddBinding("<Keyboard>/space");
        if (colorAction.bindings.Count == 0)
            colorAction.AddBinding("<Keyboard>/C");
    }

    NetworkInputData inputData = new NetworkInputData();

    private void Update()
    {
        if (shootAction.WasPressedThisFrame())
        {
            inputData.shootActionValue = true;
        }
        if (colorAction.WasPressedThisFrame())
        {
            inputData.colorActionValue = true;
        }
    }

    public override void OnInput(NetworkRunner runner, NetworkInput input)
    {
        inputData.moveActionValue = moveAction.ReadValue<Vector2>();
        input.Set(inputData); // Pass inputData by value
        inputData.shootActionValue = false; // Clear shoot flag
        inputData.colorActionValue = false; // Clear color flag
    }
}
