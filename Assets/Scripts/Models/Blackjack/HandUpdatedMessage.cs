using System;

[Serializable]
public struct HandUpdatedMessage
{
    public string PlayerId;
    public bool IsDealer;
    public Hand Hand;

    public HandUpdatedMessage(string playerId, bool isDealer, Hand hand)
    {
        PlayerId = playerId;
        IsDealer = isDealer;
        Hand = hand;
    }
}
