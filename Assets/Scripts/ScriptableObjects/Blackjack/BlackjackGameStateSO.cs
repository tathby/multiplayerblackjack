using UnityEngine;

[CreateAssetMenu(menuName = "Blackjack/Game State")]
public class BlackjackGameStateSO : ScriptableObject
{
    [SerializeField] private BlackjackGameStateData data = new BlackjackGameStateData();

    public BlackjackGameStateData Data => data;

    public void ResetState(int startingChips)
    {
        if (data.Deck.Count == 0)
        {
            data.Deck.ResetAndShuffle();
        }

        for (int i = 0; i < data.Players.Count; i++)
        {
            if (data.Players[i].Chips <= 0)
            {
                data.Players[i].Chips = startingChips;
            }
        }
        data.ResetForNewRound();
        data.Phase = GamePhase.Betting;
    }

    public void SetPhase(GamePhase phase)
    {
        data.Phase = phase;
    }

    public void ApplySnapshot(BlackjackGameStateData snapshot)
    {
        data = snapshot;
    }
}
