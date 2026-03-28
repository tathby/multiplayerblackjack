using UnityEngine;

public class StateSyncApplyController : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private StateSyncEventChannel stateSyncReceived;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private HandUpdatedEventChannel handUpdated;

    private void OnEnable()
    {
        if (stateSyncReceived != null) stateSyncReceived.OnEventRaised += ApplySnapshot;
    }

    private void OnDisable()
    {
        if (stateSyncReceived != null) stateSyncReceived.OnEventRaised -= ApplySnapshot;
    }

    private void ApplySnapshot(BlackjackGameStateData snapshot)
    {
        if (gameState == null || snapshot == null)
        {
            return;
        }

        gameState.ApplySnapshot(snapshot);
        phaseChanged?.Raise(gameState.Data.Phase);

        for (int i = 0; i < gameState.Data.Players.Count; i++)
        {
            PlayerSeatState player = gameState.Data.Players[i];
            handUpdated?.Raise(new HandUpdatedMessage(player.PlayerId, false, player.Hand));
        }

        handUpdated?.Raise(new HandUpdatedMessage("DEALER", true, gameState.Data.DealerHand));
    }
}
