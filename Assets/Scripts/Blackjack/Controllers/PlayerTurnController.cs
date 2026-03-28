using UnityEngine;

public class PlayerTurnController : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private PlayerActionEventChannel actionReceived;
    [SerializeField] private HandUpdatedEventChannel handUpdated;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private GameEvent allPlayersActed;
    [SerializeField] private GameEvent gameStateChanged;

    private void OnEnable()
    {
        if (actionReceived != null) actionReceived.OnEventRaised += OnActionReceived;
    }

    private void OnDisable()
    {
        if (actionReceived != null) actionReceived.OnEventRaised -= OnActionReceived;
    }

    private void OnActionReceived(PlayerActionMessage message)
    {
        if (!BlackjackAuthority.IsHost())
        {
            return;
        }

        if (gameState == null || gameState.Data.Phase != GamePhase.PlayerTurns)
        {
            return;
        }

        PlayerSeatState activePlayer = gameState.Data.GetActivePlayer();
        if (activePlayer == null || activePlayer.PlayerId != message.PlayerId)
        {
            return;
        }

        switch (message.Action)
        {
            case PlayerAction.Hit:
                HandleHit(activePlayer);
                break;
            case PlayerAction.Stand:
                HandleStand(activePlayer);
                break;
            case PlayerAction.DoubleDown:
                HandleDoubleDown(activePlayer);
                break;
        }

        activePlayer.UpdateDerivedState();
        handUpdated?.Raise(new HandUpdatedMessage(activePlayer.PlayerId, false, activePlayer.Hand));

        if (activePlayer.HasBusted || activePlayer.HasStood || activePlayer.HasBlackjack)
        {
            gameState.Data.AdvanceToNextPlayer();
        }

        gameStateChanged?.Raise();

        if (gameState.Data.AreAllPlayersDone())
        {
            gameState.SetPhase(GamePhase.DealerTurn);
            phaseChanged?.Raise(GamePhase.DealerTurn);
            allPlayersActed?.Raise();
        }
    }

    private void HandleHit(PlayerSeatState player)
    {
        player.Hand.AddCard(gameState.Data.Deck.Deal());
        player.UpdateDerivedState();
    }

    private void HandleStand(PlayerSeatState player)
    {
        player.HasStood = true;
    }

    private void HandleDoubleDown(PlayerSeatState player)
    {
        if (player.HasDoubled || player.Hand.Count != 2)
        {
            return;
        }

        if (player.Chips < player.Bet)
        {
            return;
        }

        player.Chips -= player.Bet;
        player.Bet *= 2;
        player.HasDoubled = true;
        player.Hand.AddCard(gameState.Data.Deck.Deal());
        player.HasStood = true;
        player.UpdateDerivedState();
    }
}
