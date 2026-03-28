using TMPro;
using UnityEngine;

public class HandTextView : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private StringVariable localPlayerId;
    [SerializeField] private HandUpdatedEventChannel handUpdated;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private bool isDealer;
    [SerializeField] private TMP_Text cardsText;
    [SerializeField] private TMP_Text valueText;

    private void OnEnable()
    {
        if (handUpdated != null) handUpdated.OnEventRaised += OnHandUpdated;
        if (phaseChanged != null) phaseChanged.OnEventRaised += OnPhaseChanged;
        Refresh();
    }

    private void OnDisable()
    {
        if (handUpdated != null) handUpdated.OnEventRaised -= OnHandUpdated;
        if (phaseChanged != null) phaseChanged.OnEventRaised -= OnPhaseChanged;
    }

    private void OnHandUpdated(HandUpdatedMessage message) => Refresh();
    private void OnPhaseChanged(GamePhase phase) => Refresh();

    private void Refresh()
    {
        if (gameState == null)
        {
            return;
        }

        if (isDealer)
        {
            RenderDealerHand();
            return;
        }

        if (localPlayerId == null)
        {
            return;
        }

        PlayerSeatState player = gameState.Data.GetPlayer(localPlayerId.Value);
        if (player == null)
        {
            if (cardsText != null) cardsText.text = "";
            if (valueText != null) valueText.text = "";
            return;
        }

        if (cardsText != null) cardsText.text = FormatHand(player.Hand, false);
        if (valueText != null) valueText.text = $"Value: {player.Hand.GetBestValue()}";
    }

    private void RenderDealerHand()
    {
        Hand dealerHand = gameState.Data.DealerHand;
        if (gameState.Data.DealerHoleCardHidden && dealerHand.Count > 1)
        {
            if (cardsText != null) cardsText.text = $"{dealerHand.Cards[0]} | ??";
            if (valueText != null) valueText.text = "Value: ??";
            return;
        }

        if (cardsText != null) cardsText.text = FormatHand(dealerHand, false);
        if (valueText != null) valueText.text = $"Value: {dealerHand.GetBestValue()}";
    }

    private string FormatHand(Hand hand, bool hideAll)
    {
        if (hand == null || hand.Count == 0)
        {
            return "";
        }

        if (hideAll)
        {
            return "??";
        }

        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < hand.Count; i++)
        {
            if (i > 0) builder.Append(" | ");
            builder.Append(hand.Cards[i]);
        }
        return builder.ToString();
    }
}
