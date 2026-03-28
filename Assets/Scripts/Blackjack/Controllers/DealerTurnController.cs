using UnityEngine;

public class DealerTurnController : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private HandUpdatedEventChannel handUpdated;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private GameEvent dealerTurnEnded;
    [SerializeField] private GameEvent gameStateChanged;
    [SerializeField] private DealerAI dealerAI;

    public void PlayDealerTurn()
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
        data.DealerHoleCardHidden = false;
        gameState.SetPhase(GamePhase.DealerTurn);
        phaseChanged?.Raise(GamePhase.DealerTurn);
        handUpdated?.Raise(new HandUpdatedMessage("DEALER", true, data.DealerHand));

        while (ShouldDealerHit(data))
        {
            data.DealerHand.AddCard(data.Deck.Deal());
            handUpdated?.Raise(new HandUpdatedMessage("DEALER", true, data.DealerHand));
        }

        gameStateChanged?.Raise();
        dealerTurnEnded?.Raise();
    }

    private bool ShouldDealerHit(BlackjackGameStateData data)
    {
        if (dealerAI != null)
        {
            return dealerAI.ShouldHit(data.DealerHand);
        }

        return data.DealerHand.GetBestValue() < 17;
    }
}
