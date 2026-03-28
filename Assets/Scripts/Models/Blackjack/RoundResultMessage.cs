using System;

[Serializable]
public struct RoundResultMessage
{
    public string PlayerId;
    public RoundOutcome Outcome;
    public int Payout;

    public RoundResultMessage(string playerId, RoundOutcome outcome, int payout)
    {
        PlayerId = playerId;
        Outcome = outcome;
        Payout = payout;
    }
}
