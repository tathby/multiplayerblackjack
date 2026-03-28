using System;

[Serializable]
public class LobbyTimetable
{
    public long createdAt;
    public long early;
    public long onTime;
    public long tooLate;
    public long dropDead;
    public long ignoreDropDeadUntil;
}