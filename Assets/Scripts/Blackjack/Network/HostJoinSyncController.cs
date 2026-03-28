using UnityEngine;

public class HostJoinSyncController : MonoBehaviour
{
    [SerializeField] private PlayerJoinEventChannel joinReceived;
    [SerializeField] private StateSyncEventChannel stateSyncRequested;
    [SerializeField] private BlackjackGameStateSO gameState;

    private void OnEnable()
    {
        if (joinReceived != null) joinReceived.OnEventRaised += OnPlayerJoined;
    }

    private void OnDisable()
    {
        if (joinReceived != null) joinReceived.OnEventRaised -= OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerJoinMessage message)
    {
        if (!BlackjackAuthority.IsHost())
        {
            return;
        }

        if (stateSyncRequested == null || gameState == null)
        {
            return;
        }

        stateSyncRequested.Raise(gameState.Data);
    }
}
