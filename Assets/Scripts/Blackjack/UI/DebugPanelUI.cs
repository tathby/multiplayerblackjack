using UnityEngine;

public class DebugPanelUI : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private StringVariable localPlayerId;
    [SerializeField] private PlayerBetEventChannel betRequested;
    [SerializeField] private GameEvent allBetsPlaced;
    [SerializeField] private GameEvent allPlayersActed;
    [SerializeField] private GameEvent dealerTurnEnded;
    [SerializeField] private GameEvent roundResetRequested;
    [SerializeField] private GameEvent gameStateChanged;
    [SerializeField] private StateSyncEventChannel stateSyncRequested;
    [SerializeField] private int debugBetAmount = 100;
    [SerializeField] private int debugChipGrant = 1000;

    public void AutoBet()
    {
        if (betRequested == null || localPlayerId == null)
        {
            return;
        }

        betRequested.Raise(new PlayerBetMessage(localPlayerId.Value, debugBetAmount));
    }

    public void ForceDeal()
    {
        if (!BlackjackAuthority.IsHost()) return;
        allBetsPlaced?.Raise();
    }

    public void ForceDealerTurn()
    {
        if (!BlackjackAuthority.IsHost()) return;
        allPlayersActed?.Raise();
    }

    public void ForcePayout()
    {
        if (!BlackjackAuthority.IsHost()) return;
        dealerTurnEnded?.Raise();
    }

    public void ResetRound()
    {
        if (!BlackjackAuthority.IsHost()) return;
        roundResetRequested?.Raise();
    }

    public void GiveChips()
    {
        if (!BlackjackAuthority.IsHost()) return;
        if (gameState == null || localPlayerId == null) return;

        PlayerSeatState player = gameState.Data.GetOrCreatePlayer(localPlayerId.Value, debugChipGrant);
        player.Chips += debugChipGrant;
        gameStateChanged?.Raise();
        stateSyncRequested?.Raise(gameState.Data);
    }
}
