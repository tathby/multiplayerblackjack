using UnityEngine;

public class PayoutController : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private RoundResultEventChannel roundResult;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private GameEvent roundEnded;
    [SerializeField] private GameEvent gameStateChanged;

    public void ResolveRound()
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
        gameState.SetPhase(GamePhase.Resolution);
        phaseChanged?.Raise(GamePhase.Resolution);

        bool dealerBlackjack = data.DealerHand.IsBlackjack();
        bool dealerBust = data.DealerHand.IsBust();
        int dealerValue = data.DealerHand.GetBestValue();

        for (int i = 0; i < data.Players.Count; i++)
        {
            PlayerSeatState player = data.Players[i];
            int payout = 0;
            RoundOutcome outcome = RoundOutcome.Lose;

            if (player.HasBusted)
            {
                outcome = RoundOutcome.Lose;
            }
            else if (dealerBlackjack)
            {
                if (player.HasBlackjack)
                {
                    outcome = RoundOutcome.Push;
                    payout = player.Bet;
                }
                else
                {
                    outcome = RoundOutcome.Lose;
                }
            }
            else if (player.HasBlackjack)
            {
                outcome = RoundOutcome.Blackjack;
                payout = player.Bet + (player.Bet * 3 / 2);
            }
            else if (dealerBust)
            {
                outcome = RoundOutcome.Win;
                payout = player.Bet * 2;
            }
            else
            {
                int playerValue = player.Hand.GetBestValue();
                if (playerValue > dealerValue)
                {
                    outcome = RoundOutcome.Win;
                    payout = player.Bet * 2;
                }
                else if (playerValue == dealerValue)
                {
                    outcome = RoundOutcome.Push;
                    payout = player.Bet;
                }
                else
                {
                    outcome = RoundOutcome.Lose;
                }
            }

            player.Chips += payout;
            roundResult?.Raise(new RoundResultMessage(player.PlayerId, outcome, payout));
        }

        gameStateChanged?.Raise();
        roundEnded?.Raise();
    }
}
