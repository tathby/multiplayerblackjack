using System;

[Serializable]
public class LobbyRules
{
    public bool allowEarlyStartWithoutMax;
    public bool forceOnTimeStartWithoutReady;
    public bool allowJoinInProgress;
    public int onTimeStartSecs;
    public bool disbandOnStart;
    public int everyReadyMinPercent;
    public int everyReadyMinNum;
    public int earliestStartSecs;
    public int tooLateSecs;
}