using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Phase Event Channel")]
public class PhaseEventChannel : ScriptableObject
{
    public event Action<GamePhase> OnEventRaised;

    public void Raise(GamePhase value)
    {
        OnEventRaised?.Invoke(value);
    }
}
