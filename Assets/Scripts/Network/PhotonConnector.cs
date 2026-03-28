using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PhotonConnector : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks
{
    private static PhotonConnector instance;
    public RealtimeClient client;

    public static System.Action RoomJoinedEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePhoton();
        }
        else
        {
            Destroy(gameObject);
        }        
    }

    private void Update()
    {
        // Must call Service every frame to process network events
        if (client != null)
        {
            client.Service();
        }
            
    }

    public static RealtimeClient GetClient()
    {
        if (instance == null) return null;
        return instance.client;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitializePhoton()
    {
        client = new RealtimeClient(Photon.Client.ConnectionProtocol.WebSocket);
        client.AddCallbackTarget(this);

        AppSettings appSettings = new AppSettings
        {
            //Photon AppId Goes here
            AppIdRealtime = "f36f5647-b4c7-413c-950d-79980cc34b55",
            Protocol = Photon.Client.ConnectionProtocol.WebSocket,
            FixedRegion = "us",
            Server = null
        };

        client.ConnectUsingSettings(appSettings);
    }

    // IConnectionCallbacks
    public void OnConnected() => Debug.Log("Photon connected!");
    public void OnConnectedToMaster() => Debug.Log("Connected to Master Server");
    public void OnDisconnected(DisconnectCause cause) => Debug.LogError("Disconnected: " + cause);
    public void OnRegionListReceived(RegionHandler regionHandler) { }
    public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }
    public void OnCustomAuthenticationFailed(string debugMessage) { }

    public void OnCreatedRoom() { }
    public void OnCreateRoomFailed(short returnCode, string message) { }
    public void OnJoinedRoom() { Debug.Log("Joined room!"); RoomJoinedEvent?.Invoke(); }
    public void OnJoinRoomFailed(short returnCode, string message) { }
    public void OnJoinRandomFailed(short returnCode, string message) { }
    public void OnLeftRoom() { }
    public void OnFriendListUpdate(List<FriendInfo> friendList) { }
}
