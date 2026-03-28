using System;

[Serializable]
public struct PlayerJoinMessage
{
    public string PlayerId;

    public PlayerJoinMessage(string playerId)
    {
        PlayerId = playerId;
    }
}
