using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Player Action Channel")]
public class PlayerActionEventChannel : ScriptableObject
{
    public event Action<PlayerActionMessage> OnEventRaised;

    public void Raise(PlayerActionMessage message)
    {
        OnEventRaised?.Invoke(message);
    }
}
