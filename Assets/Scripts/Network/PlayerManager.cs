using System.Collections.Generic;
using Photon.Realtime;
using Photon.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour, IOnEventCallback
{
    private static PlayerManager instance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PhotonConnector.GetClient().AddCallbackTarget(this);

        SessionUser[] sessionUsers = SessionManager.GetUsers();
    }

    public static void BroadcastGameover()
    {
        if (UserManager.GetUserID() != BCConfig.GetCurrentHostID()) return;

        var content = new Dictionary<string, object>
        {
            {"test", "test"}
        };
        BroadcastMessage(PhotonEventCodes.Gameover, content, ReceiverGroup.All, SendOptions.SendReliable);
    }

    private static void BroadcastMessage(PhotonEventCodes eventCode,Dictionary<string, object> content,ReceiverGroup receiverGroup,SendOptions sendOptions)
    {
        content["playerId"] = UserManager.GetUserID();

        var options = new RaiseEventArgs
        {
            Receivers = ReceiverGroup.Others
        };

        PhotonConnector.GetClient().OpRaiseEvent(
            (byte)eventCode,
            content,
            options,
            sendOptions
        );
    }

    public void OnEvent(EventData photonEvent)
    {
        //Debug.Log(photonEvent);
        var data = photonEvent.CustomData as Dictionary<string, object>;
        if (data == null) return;

        string playerId = data["playerId"].ToString();
        if (playerId == UserManager.GetUserID()) return;


        switch ((PhotonEventCodes)photonEvent.Code)
        {
            case PhotonEventCodes.Gameover:
                SceneManager.LoadScene("NetworkDemo");
                break;
            default:
                Debug.Log("invalid event code");
                break;
        }
    }
}
