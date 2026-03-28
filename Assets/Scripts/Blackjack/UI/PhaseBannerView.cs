using TMPro;
using UnityEngine;

public class PhaseBannerView : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private TMP_Text phaseText;

    private void OnEnable()
    {
        if (phaseChanged != null) phaseChanged.OnEventRaised += OnPhaseChanged;
        Refresh();
    }

    private void OnDisable()
    {
        if (phaseChanged != null) phaseChanged.OnEventRaised -= OnPhaseChanged;
    }

    private void OnPhaseChanged(GamePhase phase) => Refresh();

    private void Refresh()
    {
        if (gameState == null || phaseText == null)
        {
            return;
        }

        phaseText.text = $"Phase: {gameState.Data.Phase}";
    }
}
