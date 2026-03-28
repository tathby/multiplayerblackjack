using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Round Result Channel")]
public class RoundResultEventChannel : ScriptableObject
{
    public event Action<RoundResultMessage> OnEventRaised;

    public void Raise(RoundResultMessage message)
    {
        OnEventRaised?.Invoke(message);
    }
}
