using UnityEngine;

public class BettingController : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private PlayerBetEventChannel betReceived;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private GameEvent allBetsPlaced;
    [SerializeField] private GameEvent gameStateChanged;
    [SerializeField] private IntVariable startingChips;

    private void OnEnable()
    {
        if (betReceived != null) betReceived.OnEventRaised += OnBetReceived;
    }

    private void OnDisable()
    {
        if (betReceived != null) betReceived.OnEventRaised -= OnBetReceived;
    }

    private void OnBetReceived(PlayerBetMessage message)
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
        PlayerSeatState player = data.GetOrCreatePlayer(message.PlayerId, startingChips != null ? startingChips.Value : 1000);

        if (player.Bet > 0)
        {
            return;
        }

        int betAmount = Mathf.Clamp(message.Bet, data.MinBet, data.MaxBet);
        if (betAmount > player.Chips)
        {
            return;
        }

        player.Bet = betAmount;
        player.Chips -= betAmount;

        gameStateChanged?.Raise();

        if (data.AreAllBetsPlaced())
        {
            gameState.SetPhase(GamePhase.Dealing);
            phaseChanged?.Raise(GamePhase.Dealing);
            allBetsPlaced?.Raise();
        }
    }
}
