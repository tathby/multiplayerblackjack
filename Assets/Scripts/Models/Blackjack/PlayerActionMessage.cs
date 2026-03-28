using System;

[Serializable]
public struct PlayerActionMessage
{
    public string PlayerId;
    public PlayerAction Action;

    public PlayerActionMessage(string playerId, PlayerAction action)
    {
        PlayerId = playerId;
        Action = action;
    }
}
