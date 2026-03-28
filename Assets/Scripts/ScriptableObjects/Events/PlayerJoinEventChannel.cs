using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Player Join Channel")]
public class PlayerJoinEventChannel : ScriptableObject
{
    public event Action<PlayerJoinMessage> OnEventRaised;

    public void Raise(PlayerJoinMessage message)
    {
        OnEventRaised?.Invoke(message);
    }
}
