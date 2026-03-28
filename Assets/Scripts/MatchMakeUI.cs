using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MatchMakeUI : MonoBehaviour
{
    [SerializeField] TMP_InputField username;
    [SerializeField] TMP_InputField password;
    [SerializeField] TMP_InputField signupEmail;
    [SerializeField] TMP_InputField signupPassword;
    [SerializeField] TMP_InputField playerName;

    [SerializeField] Button authenticate;
    [SerializeField] Button signup;
    [SerializeField] Button register;
    [SerializeField] Button matchMake;

    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject signUpPanel;
    [SerializeField] GameObject matchmakePanel;
    [SerializeField] FlexibleColorPicker colorPicker;

    [SerializeField] Toggle rememberMe;

    [SerializeField] TMP_Text status;

    Tween saveColorDelay;

    private void Awake()
    {
        PhotonConnector.RoomJoinedEvent += OnRoomJoined;
        BCConfig.LobbyOperationEvent += OnLobbyOperation;
        UserManager.PropertiesLoadedEvent += OnPropertiesLoaded;
        colorPicker.onColorChange.AddListener(OnColorChanged);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        authenticate.onClick.AddListener(() => { OnAuthenticateClicked(false); });
        signup.onClick.AddListener(() => { 
            loginPanel.SetActive(false);
            signUpPanel.SetActive(true);
        });
        register.onClick.AddListener(() => { OnAuthenticateClicked(true); });
        matchMake.onClick.AddListener(OnMatchMakeClicked);

        if(PlayerPrefs.GetInt("remember", 0) == 1)
        {
            username.text = PlayerPrefs.GetString("username");
            password.text = PlayerPrefs.GetString("password");
            rememberMe.isOn = true;
            //OnAuthenticateClicked(false);
        }
    }

    void OnRoomJoined()
    {
        status.text = "Room Joined";

        PhotonConnector.RoomJoinedEvent -= OnRoomJoined;
        BCConfig.LobbyOperationEvent -= OnLobbyOperation;
        UserManager.PropertiesLoadedEvent -= OnPropertiesLoaded;

        SceneManager.LoadScene("Game");
    }

    void OnAuthenticateClicked(bool createUser)
    {
        status.text = "Logging In";

        if(rememberMe.isOn)
        {
            PlayerPrefs.SetInt("remember", 1);
            PlayerPrefs.SetString("username",username.text);
            PlayerPrefs.SetString("password",password.text);
        }
        else
        {
            PlayerPrefs.SetInt("remember", 0);
        }

        string email = createUser ? signupEmail.text : username.text;
        string pass = createUser ? signupEmail.text : password.text;

        

        BCConfig.AuthenticateAnonymous(() =>
        {
            
        }, () =>
        {
            status.text = "Log In Failed";
        });
    }

    void OnLogoutClicked()
    {
        BCConfig.Logout(() =>
        {
            loginPanel.SetActive(true);
            signUpPanel.SetActive(false);
            matchmakePanel.SetActive(false);
        });
    }

    void OnPropertiesLoaded()
    {
        status.text = "Logged In";
        loginPanel.SetActive(false);
        signUpPanel.SetActive(false);
        matchmakePanel.SetActive(true);
        colorPicker.SetColor(UserManager.GetUserColor(), true);
    }

    void OnMatchMakeClicked()
    {
        BCConfig.FindMatch();
        status.text = "Finding match.";
    }

    void OnLobbyOperation(string operation)
    {
        status.text = operation;
    }

    void OnColorChanged(Color c)
    {
        if(saveColorDelay != null)
        {
            saveColorDelay.Kill();
        }

        saveColorDelay = DOVirtual.DelayedCall(0.25f, () =>
        {
            string hexCode = string.Format("#{0}", ColorUtility.ToHtmlStringRGB(c));
            UserManager.SetColor(hexCode);
            saveColorDelay = null;
        });
        
    }
}
