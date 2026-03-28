using UnityEngine;

public class DealController : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private HandUpdatedEventChannel handUpdated;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private GameEvent allPlayersActed;
    [SerializeField] private GameEvent gameStateChanged;

    public void BeginDeal()
    {
        if (!BlackjackAuthority.IsHost())
        {
            return;
        }

        if (gameState == null)
        {
            return;
        }

        BlackjackGameStateData data = gameState.Data;
        data.ResetForNewRound();
        data.Phase = GamePhase.Dealing;

        for (int i = 0; i < data.Players.Count; i++)
        {
            PlayerSeatState player = data.Players[i];
            player.Hand.AddCard(data.Deck.Deal());
            player.Hand.AddCard(data.Deck.Deal());
            player.UpdateDerivedState();
            handUpdated?.Raise(new HandUpdatedMessage(player.PlayerId, false, player.Hand));
        }

        data.DealerHand.AddCard(data.Deck.Deal());
        data.DealerHand.AddCard(data.Deck.Deal());
        handUpdated?.Raise(new HandUpdatedMessage("DEALER", true, data.DealerHand));

        data.ActivePlayerIndex = 0;
        if (data.GetActivePlayer() != null && data.GetActivePlayer().HasBlackjack)
        {
            data.AdvanceToNextPlayer();
        }

        gameStateChanged?.Raise();

        if (data.AreAllPlayersDone())
        {
            gameState.SetPhase(GamePhase.DealerTurn);
            phaseChanged?.Raise(GamePhase.DealerTurn);
            allPlayersActed?.Raise();
            return;
        }

        gameState.SetPhase(GamePhase.PlayerTurns);
        phaseChanged?.Raise(GamePhase.PlayerTurns);
    }
}
