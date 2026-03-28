using System;

[Serializable]
public struct PlayerBetMessage
{
    public string PlayerId;
    public int Bet;

    public PlayerBetMessage(string playerId, int bet)
    {
        PlayerId = playerId;
        Bet = bet;
    }
}
