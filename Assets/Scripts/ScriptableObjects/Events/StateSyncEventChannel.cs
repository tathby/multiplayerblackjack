using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/State Sync Channel")]
public class StateSyncEventChannel : ScriptableObject
{
    public event Action<BlackjackGameStateData> OnEventRaised;

    public void Raise(BlackjackGameStateData message)
    {
        OnEventRaised?.Invoke(message);
    }
}
