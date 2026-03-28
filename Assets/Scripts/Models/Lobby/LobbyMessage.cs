using System;

[Serializable]
public class LobbyMessage
{
    public string service;
    public string operation;
    public LobbyMessageData data;
}