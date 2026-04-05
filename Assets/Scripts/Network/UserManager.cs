using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    private static UserManager instance;
    PlayerDataResponse.PlayerDataWrapper user;
    PlayerPropertiesResponse.PlayerProperties properties;

    public static System.Action PropertiesLoadedEvent;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        BCConfig.LoginEvent += OnLogin;
    }

    void OnLogin(PlayerDataResponse response)
    {
        user = response.data;

        BCConfig.LoadUserProperties(p =>
        {
            properties = p;
            PropertiesLoadedEvent?.Invoke();
        });
    }

    public static void SetColor(string hex)
    {
        instance.p_SetColor(hex);   
    }

    public static string GetUserID()
    {
        return instance.user.id;
    }

    public static bool TryGetUserID(out string userId)
    {
        userId = null;
        if (instance == null || instance.user == null)
        {
            return false;
        }

        userId = instance.user.id;
        return !string.IsNullOrEmpty(userId);
    }

    public static Color GetUserColor()
    {
        if (ColorUtility.TryParseHtmlString(instance.properties.colorHex, out Color color))
        {
            return color;
        }

        return Color.white;
    }

    public static Dictionary<string,object> GetAttributesDict()
    {
        return instance.properties.ToDictionary();
    }

    void p_SetColor(string hex)
    {
        properties.colorHex = hex;
        //You can save data that you want other players to be able to access by
        //assigning User Properties. The data will be given to other players when joining
        //the lobby
        BCConfig.SaveUserProperty("colorHex", hex);
    }
}
