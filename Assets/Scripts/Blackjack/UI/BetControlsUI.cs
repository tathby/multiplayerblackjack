using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetControlsUI : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private StringVariable localPlayerId;
    [SerializeField] private PlayerBetEventChannel betRequested;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private RoundResultEventChannel roundResult;
    [SerializeField] private Slider betSlider;
    [SerializeField] private Button betButton;
    [SerializeField] private TMP_Text betAmountText;

    private void OnEnable()
    {
        if (betSlider != null) betSlider.onValueChanged.AddListener(OnSliderChanged);
        if (betButton != null) betButton.onClick.AddListener(OnBetClicked);
        if (phaseChanged != null) phaseChanged.OnEventRaised += OnPhaseChanged;
        if (roundResult != null) roundResult.OnEventRaised += OnRoundResult;
        Refresh();
    }

    private void OnDisable()
    {
        if (betSlider != null) betSlider.onValueChanged.RemoveListener(OnSliderChanged);
        if (betButton != null) betButton.onClick.RemoveListener(OnBetClicked);
        if (phaseChanged != null) phaseChanged.OnEventRaised -= OnPhaseChanged;
        if (roundResult != null) roundResult.OnEventRaised -= OnRoundResult;
    }

    private void OnPhaseChanged(GamePhase phase) => Refresh();
    private void OnRoundResult(RoundResultMessage message) => Refresh();

    private void OnSliderChanged(float value)
    {
        if (betAmountText != null)
        {
            betAmountText.text = $"Bet: {Mathf.RoundToInt(value)}";
        }
    }

    private void OnBetClicked()
    {
        if (betRequested == null || gameState == null || localPlayerId == null)
        {
            return;
        }

        int betValue = betSlider != null ? Mathf.RoundToInt(betSlider.value) : gameState.Data.MinBet;
        betRequested.Raise(new PlayerBetMessage(localPlayerId.Value, betValue));
    }

    private void Refresh()
    {
        if (gameState == null || betSlider == null || betButton == null)
        {
            return;
        }

        BlackjackGameStateData data = gameState.Data;
        PlayerSeatState player = data.GetPlayer(localPlayerId != null ? localPlayerId.Value : string.Empty);
        int chips = player != null ? player.Chips : data.MaxBet;
        int maxBet = Mathf.Clamp(chips, data.MinBet, data.MaxBet);

        betSlider.minValue = data.MinBet;
        betSlider.maxValue = maxBet;
        betSlider.value = Mathf.Clamp(betSlider.value, betSlider.minValue, betSlider.maxValue);
        OnSliderChanged(betSlider.value);

        bool canBet = data.Phase == GamePhase.Betting && chips >= data.MinBet && (player == null || player.Bet == 0);
        betButton.interactable = canBet;
    }
}
