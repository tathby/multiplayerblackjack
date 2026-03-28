using System;

[Serializable]
public class Lobby
{
    public string state;
    public int rating;
    public string ownerCxId;
    public LobbyTypeDef lobbyTypeDef;
    public LobbySettings settings;
    public int version;
    public LobbyTimetable timetable;
    public string[] cRegions;
    public int round;
    public bool isRoomReady;
    public int keepAliveRateSeconds;
    public bool isAvailable;
    public int shardId;
    public bool legacyLobbyOwnerEnabled;
    public int numMembers;
    public Member[] members;
}