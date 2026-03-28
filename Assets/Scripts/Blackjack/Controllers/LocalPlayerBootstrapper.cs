using UnityEngine;

public class LocalPlayerBootstrapper : MonoBehaviour
{
    [SerializeField] private StringVariable localPlayerId;
    [SerializeField] private PlayerJoinEventChannel playerJoinRequested;

    private void Start()
    {
        string playerId = UserManager.GetUserID();
        if (localPlayerId != null)
        {
            localPlayerId.SetValue(playerId);
        }

        if (playerJoinRequested != null)
        {
            playerJoinRequested.Raise(new PlayerJoinMessage(playerId));
        }
    }
}
