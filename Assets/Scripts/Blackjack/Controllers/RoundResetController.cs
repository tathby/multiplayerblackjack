using UnityEngine;

public class RoundResetController : MonoBehaviour
{
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private PhaseEventChannel phaseChanged;
    [SerializeField] private IntVariable startingChips;

    public void StartNewRound()
    {
        if (!BlackjackAuthority.IsHost())
        {
            return;
        }

        if (gameState == null)
        {
            return;
        }

        int chips = startingChips != null ? startingChips.Value : 1000;
        gameState.ResetState(chips);
        phaseChanged?.Raise(GamePhase.Betting);
    }
}
