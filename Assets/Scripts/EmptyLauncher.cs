// From the Fusion 2 Tutorial: https://doc.photonengine.com/fusion/current/tutorials/host-mode-basics/2-setting-up-a-scene#launching-fusion
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// This class demonstrates the most basic procedure for launching Fusion NetworkRunner.
// INetworkRunnerCallbacks is an interface that contains all "On*" methods relevant to connecting to Fusion Network Runner.
public class EmptyLauncher: MonoBehaviour, INetworkRunnerCallbacks {
    public virtual void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        Debug.Log($"OnPlayerJoined {player}");
    }
    public virtual void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        Debug.Log($"OnPlayerLeft {player}");
    }
    public virtual void OnInput(NetworkRunner runner, NetworkInput input) {
        Debug.Log($"OnInput {input}");
    }
    public virtual void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
        Debug.Log($"OnInputMissing {player} {input}");
    }
    public virtual void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
        Debug.Log($"OnShutdown reason={shutdownReason}");
    }
    public virtual void OnConnectedToServer(NetworkRunner runner) {
        Debug.Log($"OnConnectedToServer");
    }
    public virtual void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {
        Debug.Log($"OnDisconnectedFromServer {reason}");
    }
    public virtual void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {
        Debug.Log($"OnConnectRequest {request} {token}");
    }
    public virtual void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
        Debug.Log($"OnConnectFailed {remoteAddress} {reason}");
    }

    public virtual void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {
        Debug.Log($"OnUserSimulationMessage {message}");
    }
    public virtual void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        Debug.Log($"OnSessionListUpdated {sessionList}");
    }
    public virtual void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {
        Debug.Log($"OnSessionListUpdated {data}");
    }
    public virtual void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {
        Debug.Log($"OnHostMigration {hostMigrationToken}");
    }
    public virtual void OnSceneLoadDone(NetworkRunner runner) {
        Debug.Log($"OnSceneLoadDone");
    }
    public virtual void OnSceneLoadStart(NetworkRunner runner) { 
        Debug.Log($"OnSceneLoadStart");
    }
    public virtual void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        Debug.Log($"OnObjectExitAOI {obj} {player}");
    }

    public virtual void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        Debug.Log($"OnObjectExitAOI {obj} {player}");
    }
    public virtual void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {
        Debug.Log($"OnReliableDataReceived {player} {key} {data}");
    }
    public virtual void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {
        Debug.Log($"OnReliableDataReceived {player} {key} {progress}");
    }

    protected NetworkRunner _runner;

    protected async void StartGame(GameMode mode, string sessionName) {
        Debug.Log($"Starting game at mode {mode}, session {sessionName}");
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        //gameObject.AddComponent<RunnerSimulatePhysics3D>();  
            // Needed only when using NetworkRigidBody.
            // Requires the Physics add-on: https://doc.photonengine.com/fusion/current/addons/physics/download

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs() {
            GameMode = mode,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    string SESSION_NAME = "TestRoom";

    // Create and run a simple GUI that allows the player to choose whether to host a new game or join an existing game.
    protected void OnGUI() {
        if (_runner == null) {

            // From Microsoft Copilot
            GUIStyle style = new GUIStyle(GUI.skin.button); 
            style.fontSize = 25; 
            style.normal.textColor = Color.white; 
            style.hover.textColor = Color.yellow; 
            style.normal.background = MakeTex(600, 400, new Color(0.3f, 0.5f, 0.7f, 1.0f)); 
            style.hover.background = MakeTex(600, 400, new Color(0.4f, 0.6f, 0.8f, 1.0f)); 
            style.border = new RectOffset(12, 12, 12, 12);

            int width=100, height=50;

            if (GUI.Button(new Rect(0, 0, width, height), "Host", style)) {
                StartGame(GameMode.Host, SESSION_NAME);    // This mode requires Internet connection (to the Fusion cloud).
            }
            if (GUI.Button(new Rect(0, 1* height, width, height), "Client", style)) {
                StartGame(GameMode.Client, SESSION_NAME);  // This mode requires Internet connection (to the Fusion cloud).
            }
            if (GUI.Button(new Rect(0, 2* height, width, height), "1 Player", style)) {
                StartGame(GameMode.Single, SESSION_NAME);  // This mode does not require Internet connection
            }
            if (GUI.Button(new Rect(0, 3* height, width, height), "Shared", style)) {
                StartGame(GameMode.Shared, SESSION_NAME);  // This mode does not require Internet connection
            }

        }
    }

    // From Microsoft Copilot
    private Texture2D MakeTex(int width, int height, Color col) { 
        Color[] pix = new Color[width * height]; 
        for (int i = 0; i < pix.Length; i++) { pix[i] = col; } 
        Texture2D result = new Texture2D(width, height); 
        result.SetPixels(pix); 
        result.Apply(); 
        return result; 
    }
}

