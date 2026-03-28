using TMPro;
using UnityEngine;

public class PlayerChipsView : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private StringVariable localPlayerId;
    [SerializeField] private PlayerBetEventChannel betReceived;
    [SerializeField] private RoundResultEventChannel roundResult;
    [SerializeField] private TMP_Text chipsText;
    [SerializeField] private TMP_Text betText;

    private void OnEnable()
    {
        if (betReceived != null) betReceived.OnEventRaised += OnBetReceived;
        if (roundResult != null) roundResult.OnEventRaised += OnRoundResult;
        Refresh();
    }

    private void OnDisable()
    {
        if (betReceived != null) betReceived.OnEventRaised -= OnBetReceived;
        if (roundResult != null) roundResult.OnEventRaised -= OnRoundResult;
    }

    private void OnBetReceived(PlayerBetMessage message) => Refresh();
    private void OnRoundResult(RoundResultMessage message) => Refresh();

    private void Refresh()
    {
        if (gameState == null || localPlayerId == null)
        {
            return;
        }

        PlayerSeatState player = gameState.Data.GetPlayer(localPlayerId.Value);
        if (player == null)
        {
            if (chipsText != null) chipsText.text = "Chips: --";
            if (betText != null) betText.text = "Bet: --";
            return;
        }

        if (chipsText != null) chipsText.text = $"Chips: {player.Chips}";
        if (betText != null) betText.text = $"Bet: {player.Bet}";
    }
}
