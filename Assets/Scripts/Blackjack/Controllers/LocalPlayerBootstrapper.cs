using UnityEngine;

public class LocalPlayerBootstrapper : MonoBehaviour
{
    [SerializeField] private StringVariable localPlayerId;
    [SerializeField] private PlayerJoinEventChannel playerJoinRequested;

    private void Start()
    {
        string playerId;
        if (!UserManager.TryGetUserID(out playerId))
        {
            playerId = PlayerPrefs.GetString("BlackjackLocalPlayerId", string.Empty);
            if (string.IsNullOrEmpty(playerId))
            {
                playerId = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString("BlackjackLocalPlayerId", playerId);
            }
        }
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
