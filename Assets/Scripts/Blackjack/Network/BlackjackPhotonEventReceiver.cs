using Photon.Client;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackPhotonEventReceiver : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private PlayerBetEventChannel betReceived;
    [SerializeField] private PlayerActionEventChannel actionReceived;
    [SerializeField] private PlayerJoinEventChannel joinReceived;
    [SerializeField] private StateSyncEventChannel stateSyncReceived;

    private void OnEnable()
    {
        RealtimeClient client = PhotonConnector.GetClient();
        if (client != null)
        {
            client.AddCallbackTarget(this);
        }
    }

    private void OnDisable()
    {
        RealtimeClient client = PhotonConnector.GetClient();
        if (client != null)
        {
            client.RemoveCallbackTarget(this);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.CustomData is not Dictionary<string, object> data)
        {
            return;
        }

        PhotonEventCodes code = (PhotonEventCodes)photonEvent.Code;
        switch (code)
        {
            case PhotonEventCodes.PlayerBet:
                betReceived?.Raise(BlackjackPhotonSerializer.DeserializeBet(data));
                break;
            case PhotonEventCodes.PlayerAction:
                actionReceived?.Raise(BlackjackPhotonSerializer.DeserializeAction(data));
                break;
            case PhotonEventCodes.PlayerJoin:
                joinReceived?.Raise(BlackjackPhotonSerializer.DeserializeJoin(data));
                break;
            case PhotonEventCodes.StateSync:
                stateSyncReceived?.Raise(BlackjackPhotonSerializer.DeserializeState(data));
                break;
        }
    }
}
