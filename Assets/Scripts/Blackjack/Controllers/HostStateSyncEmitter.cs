using UnityEngine;

public class HostStateSyncEmitter : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private StateSyncEventChannel stateSyncRequested;

    public void EmitSync()
    {
        if (!BlackjackAuthority.IsHost())
        {
            return;
        }

        if (gameState == null || stateSyncRequested == null)
        {
            return;
        }

        stateSyncRequested.Raise(gameState.Data);
    }
}
