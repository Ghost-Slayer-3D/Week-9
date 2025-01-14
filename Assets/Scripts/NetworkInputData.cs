// From the Fusion tutorial:
// https://doc.photonengine.com/fusion/current/tutorials/host-mode-basics/2-setting-up-a-scene#launching-fusion

using Fusion;
using UnityEngine;

/**
 * When a player wants to move his character, he must send the movement information to the server.
 * The information is sent using this structure.
 * 
 * NOTE: in C#, a struct is passed by value (in contrast to a class, that is passed by reference).
 */
public struct NetworkInputData : INetworkInput
{
    public Vector2 moveActionValue;
    public bool shootActionValue;
    public bool colorActionValue;
}
