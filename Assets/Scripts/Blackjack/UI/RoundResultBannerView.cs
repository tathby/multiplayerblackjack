using TMPro;
using UnityEngine;

public class RoundResultBannerView : MonoBehaviour
{
    [SerializeField] private StringVariable localPlayerId;
    [SerializeField] private RoundResultEventChannel roundResult;
    [SerializeField] private TMP_Text resultText;

    private void OnEnable()
    {
        if (roundResult != null) roundResult.OnEventRaised += OnRoundResult;
    }

    private void OnDisable()
    {
        if (roundResult != null) roundResult.OnEventRaised -= OnRoundResult;
    }

    private void OnRoundResult(RoundResultMessage message)
    {
        if (localPlayerId == null || message.PlayerId != localPlayerId.Value || resultText == null)
        {
            return;
        }

        resultText.text = $"{message.Outcome} (+{message.Payout})";
    }
}
