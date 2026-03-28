using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Player Bet Channel")]
public class PlayerBetEventChannel : ScriptableObject
{
    public event Action<PlayerBetMessage> OnEventRaised;

    public void Raise(PlayerBetMessage message)
    {
        OnEventRaised?.Invoke(message);
    }
}
