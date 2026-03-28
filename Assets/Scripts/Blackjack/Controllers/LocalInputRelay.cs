using UnityEngine;

public class LocalInputRelay : MonoBehaviour
{
    [SerializeField] private PlayerBetEventChannel betRequested;
    [SerializeField] private PlayerBetEventChannel betReceived;
    [SerializeField] private PlayerActionEventChannel actionRequested;
    [SerializeField] private PlayerActionEventChannel actionReceived;
    [SerializeField] private PlayerJoinEventChannel joinRequested;
    [SerializeField] private PlayerJoinEventChannel joinReceived;

    private void OnEnable()
    {
        if (betRequested != null) betRequested.OnEventRaised += OnBetRequested;
        if (actionRequested != null) actionRequested.OnEventRaised += OnActionRequested;
        if (joinRequested != null) joinRequested.OnEventRaised += OnJoinRequested;
    }

    private void OnDisable()
    {
        if (betRequested != null) betRequested.OnEventRaised -= OnBetRequested;
        if (actionRequested != null) actionRequested.OnEventRaised -= OnActionRequested;
        if (joinRequested != null) joinRequested.OnEventRaised -= OnJoinRequested;
    }

    private void OnBetRequested(PlayerBetMessage message)
    {
        betReceived?.Raise(message);
    }

    private void OnActionRequested(PlayerActionMessage message)
    {
        actionReceived?.Raise(message);
    }

    private void OnJoinRequested(PlayerJoinMessage message)
    {
        joinReceived?.Raise(message);
    }
}
