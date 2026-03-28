using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Hand Updated Channel")]
public class HandUpdatedEventChannel : ScriptableObject
{
    public event Action<HandUpdatedMessage> OnEventRaised;

    public void Raise(HandUpdatedMessage message)
    {
        OnEventRaised?.Invoke(message);
    }
}
