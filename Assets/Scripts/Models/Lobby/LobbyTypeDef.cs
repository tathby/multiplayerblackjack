
using System;

[Serializable]
public class LobbyTypeDef
{
    public string roomConfig;  // null allowed
    public string lobbyTypeId;
    public LobbyTeams teams;
    public LobbyRules rules;
    public string desc;
}