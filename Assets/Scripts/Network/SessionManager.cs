// Keeps track of ids of other users in the session

using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    private static SessionManager instance;
    private List<SessionUser> users = new List<SessionUser>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        BCConfig.PlayerJoinedEvent += OnPlayerJoined;
    }

    void OnPlayerJoined(SessionUser user)
    {
        if (users.Exists(u => u.PlayerId == user.PlayerId)) return;
        if (user.PlayerId == UserManager.GetUserID()) return;
        
        users.Add(user);
    }

    public static SessionUser[] GetUsers()
    {
        return instance.users.ToArray();
    }

}

public class SessionUser
{
    public string PlayerId { get; }
    public string PlayerName { get; }
    public PlayerPropertiesResponse.PlayerProperties Attributes { get; }

    public SessionUser(Member member)
    {
        PlayerId = member.profileId;
        PlayerName = member.name;
        Attributes = member.extra.attributes;
    }

    public SessionUser(string pid, string playerName, PlayerPropertiesResponse.PlayerProperties attributes)
    {
        PlayerId = pid;
        PlayerName = playerName;
        Attributes = attributes;
    }
}

