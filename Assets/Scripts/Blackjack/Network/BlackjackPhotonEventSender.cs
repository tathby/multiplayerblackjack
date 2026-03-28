using Photon.Client;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackPhotonEventSender : MonoBehaviour
{
    [SerializeField] private PlayerBetEventChannel betRequested;
    [SerializeField] private PlayerActionEventChannel actionRequested;
    [SerializeField] private PlayerJoinEventChannel joinRequested;
    [SerializeField] private StateSyncEventChannel stateSyncRequested;

    private void OnEnable()
    {
        if (betRequested != null) betRequested.OnEventRaised += SendBet;
        if (actionRequested != null) actionRequested.OnEventRaised += SendAction;
        if (joinRequested != null) joinRequested.OnEventRaised += SendJoin;
        if (stateSyncRequested != null) stateSyncRequested.OnEventRaised += SendStateSync;
    }

    private void OnDisable()
    {
        if (betRequested != null) betRequested.OnEventRaised -= SendBet;
        if (actionRequested != null) actionRequested.OnEventRaised -= SendAction;
        if (joinRequested != null) joinRequested.OnEventRaised -= SendJoin;
        if (stateSyncRequested != null) stateSyncRequested.OnEventRaised -= SendStateSync;
    }

    private void SendBet(PlayerBetMessage message)
    {
        RaiseEvent(PhotonEventCodes.PlayerBet, BlackjackPhotonSerializer.SerializeBet(message));
    }

    private void SendAction(PlayerActionMessage message)
    {
        RaiseEvent(PhotonEventCodes.PlayerAction, BlackjackPhotonSerializer.SerializeAction(message));
    }

    private void SendJoin(PlayerJoinMessage message)
    {
        RaiseEvent(PhotonEventCodes.PlayerJoin, BlackjackPhotonSerializer.SerializeJoin(message));
    }

    private void SendStateSync(BlackjackGameStateData state)
    {
        RaiseEvent(PhotonEventCodes.StateSync, BlackjackPhotonSerializer.SerializeState(state));
    }

    private void RaiseEvent(PhotonEventCodes code, Dictionary<string, object> payload)
    {
        RealtimeClient client = PhotonConnector.GetClient();
        if (client == null)
        {
            Debug.LogWarning("Photon client not ready for event send.");
            return;
        }

        var options = new RaiseEventArgs
        {
            Receivers = ReceiverGroup.Others
        };

        client.OpRaiseEvent((byte)code, payload, options, SendOptions.SendReliable);
    }
}
