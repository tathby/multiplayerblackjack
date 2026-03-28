using System;

[Serializable]
public class LobbyMessageData
{
    public string lobbyId;
    public long currentTime;
    public Lobby lobby;
    public Member member;
}