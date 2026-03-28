using System.Collections.Generic;
using Photon.Realtime;

public class BrainCloudPhotonBridge
{
    /// <summary>
    /// Call this when BrainCloud matchmaking returns room info.
    /// </summary>
    public void OnBrainCloudMatchFound(Dictionary<string, object> roomData)
    {
        if (!roomData.ContainsKey("roomName"))
        {
            UnityEngine.Debug.LogError("[BrainCloud] Room info missing 'roomName'");
            return;
        }

        string roomName = roomData["roomName"].ToString();

        JoinPhotonRoom(roomName);
    }

    public void JoinPhotonRoom(string roomName)
    {
        if (PhotonConnector.GetClient() == null)
        {
            UnityEngine.Debug.LogError("PhotonManager not initialized");
            return;
        }

        var roomOptions = new RoomOptions
        {
            MaxPlayers = 8,   // adjust for your game
            IsVisible = true,
            IsOpen = true
        };

        PhotonConnector.GetClient().OpJoinOrCreateRoom(new EnterRoomArgs
        {
            RoomName = roomName,
            RoomOptions = roomOptions,
            Lobby = null
        });

        UnityEngine.Debug.Log("[Photon] Attempting to join or create room: " + roomName);
    }
}
