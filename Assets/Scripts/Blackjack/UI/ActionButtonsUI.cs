using UnityEngine;
using UnityEngine.UI;

public class ActionButtonsUI : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private StringVariable localPlayerId;
    [SerializeField] private PlayerActionEventChannel actionRequested;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private HandUpdatedEventChannel handUpdated;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button doubleButton;

    private void OnEnable()
    {
        if (hitButton != null) hitButton.onClick.AddListener(() => SendAction(PlayerAction.Hit));
        if (standButton != null) standButton.onClick.AddListener(() => SendAction(PlayerAction.Stand));
        if (doubleButton != null) doubleButton.onClick.AddListener(() => SendAction(PlayerAction.DoubleDown));
        if (phaseChanged != null) phaseChanged.OnEventRaised += OnPhaseChanged;
        if (handUpdated != null) handUpdated.OnEventRaised += OnHandUpdated;
        Refresh();
    }

    private void OnDisable()
    {
        if (hitButton != null) hitButton.onClick.RemoveAllListeners();
        if (standButton != null) standButton.onClick.RemoveAllListeners();
        if (doubleButton != null) doubleButton.onClick.RemoveAllListeners();
        if (phaseChanged != null) phaseChanged.OnEventRaised -= OnPhaseChanged;
        if (handUpdated != null) handUpdated.OnEventRaised -= OnHandUpdated;
    }

    private void OnPhaseChanged(GamePhase phase) => Refresh();
    private void OnHandUpdated(HandUpdatedMessage message) => Refresh();

    private void SendAction(PlayerAction action)
    {
        if (actionRequested == null || localPlayerId == null)
        {
            return;
        }

        actionRequested.Raise(new PlayerActionMessage(localPlayerId.Value, action));
    }

    private void Refresh()
    {
        if (gameState == null || localPlayerId == null)
        {
            return;
        }

        BlackjackGameStateData data = gameState.Data;
        bool isMyTurn = data.Phase == GamePhase.PlayerTurns;
        PlayerSeatState active = data.GetActivePlayer();
        if (active == null || active.PlayerId != localPlayerId.Value)
        {
            isMyTurn = false;
        }

        if (hitButton != null) hitButton.interactable = isMyTurn;
        if (standButton != null) standButton.interactable = isMyTurn;

        bool canDouble = false;
        PlayerSeatState localPlayer = data.GetPlayer(localPlayerId.Value);
        if (localPlayer != null)
        {
            canDouble = isMyTurn && localPlayer.Hand.Count == 2 && !localPlayer.HasDoubled && localPlayer.Chips >= localPlayer.Bet;
        }

        if (doubleButton != null) doubleButton.interactable = canDouble;
    }
}
