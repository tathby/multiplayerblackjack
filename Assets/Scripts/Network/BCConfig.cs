using BrainCloud;
using System;
using System.Collections;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using UnityEngine;
using Photon.Realtime;

public class BCConfig : MonoBehaviour
{
    private static BCConfig instance;
    private BrainCloudPhotonBridge photonBridge;
    private BrainCloudWrapper _bc;

    public PlayerDataResponse PlayerDataResponse;
    public string currentUserId;
    public string lobbyId;
    public string ownerId;

    public static System.Action<short, byte[]> NetworkCallbackEvent;
    public static Action<SessionUser> PlayerJoinedEvent;
    public static Action<PlayerDataResponse> LoginEvent;
    public static Action<string> LobbyOperationEvent;

    public static BrainCloudWrapper GetBrainCloud()
    {
        return instance._bc;
    }

    // Use this for initialization
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        _bc = gameObject.AddComponent<BrainCloudWrapper>();
        
        _bc.WrapperName = gameObject.name;    // Optional: Set a wrapper name
        _bc.Init();      // Init data is taken from the brainCloud Unity Plugin

        EnableRTT();

        photonBridge = new BrainCloudPhotonBridge();
    }

    public static string GetPlayerID()
    {
        return instance.PlayerDataResponse.data.id;
    }

    [ContextMenu("Authenticate Anonymous")]
    public static void AuthenticateAnonymous(System.Action onComplete, System.Action onFail)
    {
        instance._bc.AuthenticateAnonymous((jsonResponse, cbObject) =>
        {

            instance._bc.RTTService.EnableRTT(
                (response, cbObject) =>
                {
                    Debug.Log("RTT CONNECTED: " + response);
                },
                (status, reasonCode, jsonError, cbObject) =>
                {
                    Debug.LogError($"RTT FAILED: status={status}, reason={reasonCode}, error={jsonError}");
                }
            );

            instance.LoginSuccessCallback(jsonResponse, cbObject);
            onComplete();
        }, (int status, int reasonCode, string jsonError, object cbObject) =>
        {
            onFail();
        });
    }

    public static void AuthenticateEmail(string email, string pass, string name,bool createUser, System.Action onComplete, System.Action onFail)
    {        
        instance._bc.AuthenticateEmailPassword(email, pass, createUser, (jsonResponse, cbObject) =>
        {
            var response = JsonReader.Deserialize<Dictionary<string, object>>(jsonResponse);
            var data = response["data"] as Dictionary<string, object>;

            bool isNewUser = false;
            if (data.ContainsKey("newUser"))
            {
                isNewUser = data["newUser"].ToString() != "false";

                if(isNewUser) SetPlayerName(name);
            }

            instance._bc.RTTService.EnableRTT(
                (response, cbObject) =>
                {
                    Debug.Log("RTT CONNECTED: " + response);
                },
                (status, reasonCode, jsonError, cbObject) =>
                {
                    Debug.LogError($"RTT FAILED: status={status}, reason={reasonCode}, error={jsonError}");
                }
            );

            instance.LoginSuccessCallback(jsonResponse, cbObject);
            onComplete();
        }, (int status, int reasonCode, string jsonError, object cbObject) =>
        {
            onFail();
        });
    }

    public static void Logout(System.Action onComplete)
    {
        instance._bc.Logout(true, (jsonResponse, cbObject) =>
        {
            instance._bc.RTTService.DisableRTT();
        });
    }

    public static void SetPlayerName(string name)
    {
        instance._bc.PlayerStateService.UpdateName(name,
            (resp, _) =>
            {
                Debug.Log("Player name set to: " + name);
            },
            (status, reason, error, _) =>
            {
                Debug.LogError("Failed to set player name: " + error);
            });
    }

    public static void FindMatch()
    {
        instance.FindLobby();
    }

    [ContextMenu("Enable RTT")]
    public void EnableRTT()
    {
        _bc.RTTService.RegisterRTTLobbyCallback((jsonMessage) =>
        {
            Debug.Log("[LOBBY EVENT] " + jsonMessage);

            var msg = JsonReader.Deserialize<Dictionary<string, object>>(jsonMessage);
            if (msg == null || !msg.ContainsKey("operation") || !msg.ContainsKey("data"))
                return;


            string operation = msg["operation"].ToString();

            LobbyOperationEvent?.Invoke(operation);

            if (operation == "MEMBER_JOIN")
            {
                LobbyMessage lobbyMessage = JsonUtility.FromJson<LobbyMessage>(jsonMessage);

                if (lobbyMessage == null) return;
                if (lobbyMessage.data == null) return;
                if (lobbyMessage.data.lobby == null) return;
                if (lobbyMessage.data.lobby.members == null) return;

                Member[] members = lobbyMessage.data.lobby.members;

                for (int i = 0; i < members.Length; i++)
                {
                    SessionUser user = new SessionUser(members[i]);
                    PlayerJoinedEvent?.Invoke(user);
                }

                ownerId = lobbyMessage.data.lobby.ownerCxId.Split(':')[1];

                //{"service":"lobby","operation":"MEMBER_JOIN","data":{"lobbyId":"15495:Default:18","currentTime":1763666634739,"lobby":{"state":"setup","rating":0,"ownerCxId":"15495:ebc4c49f-ece4-4c41-a707-e209a6bb530f:3fne4t3vjretpsj0rek8b43kr1","lobbyTypeDef":{"roomConfig":null,"lobbyTypeId":"Default","teams":{"all":{"minUsers":1,"maxUsers":8,"autoAssign":true,"code":"all"}},"rules":{"allowEarlyStartWithoutMax":true,"forceOnTimeStartWithoutReady":true,"allowJoinInProgress":false,"onTimeStartSecs":120,"disbandOnStart":true,"everyReadyMinPercent":50,"everyReadyMinNum":1,"earliestStartSecs":30,"tooLateSecs":300},"desc":"Default Test Lobby"},"settings":{},"version":1,"timetable":{"createdAt":1763666634739,"early":1763666664739,"onTime":1763666754739,"tooLate":1763666934739,"dropDead":1763667534739,"ignoreDropDeadUntil":0},"cRegions":[],"round":1,"isRoomReady":false,"keepAliveRateSeconds":0,"isAvailable":true,"shardId":0,"legacyLobbyOwnerEnabled":false,"numMembers":1,"members":[{"profileId":"ebc4c49f-ece4-4c41-a707-e209a6bb530f","name":"","pic":"","rating":0,"team":"all","isReady":true,"extra":{},"ipAddress":"205.178.105.16","cxId":"15495:ebc4c49f-ece4-4c41-a707-e209a6bb530f:3fne4t3vjretpsj0rek8b43kr1"}]},"member":{"profileId":"ebc4c49f-ece4-4c41-a707-e209a6bb530f","name":"","pic":"","rating":0,"team":"all","isReady":true,"extra":{},"ipAddress":"205.178.105.16","cxId":"15495:ebc4c49f-ece4-4c41-a707-e209a6bb530f:3fne4t3vjretpsj0rek8b43kr1"}}}

                var data = msg["data"] as Dictionary<string, object>;
                if (data == null)
                    return;

                string lobbyId = data["lobbyId"].ToString();
                Debug.Log("[BC] Lobby ready: " + lobbyId);

                photonBridge.JoinPhotonRoom(lobbyId);
            }
            //else if(operation == "ROOM_READY")
            else if (operation == "STARTING")
            {
                /*var data = msg["data"] as Dictionary<string, object>;
                if (data == null)
                    return;

                string lobbyId = data["lobbyId"].ToString();
                Debug.Log("[BC] Lobby ready: " + lobbyId);

                photonBridge.JoinPhotonRoom(lobbyId);*/
            }
            

        });        

    }

    public static string GetCurrentHostID()
    {
        return instance.ownerId;
    }

    public static void SaveUserProperty(string key, object val)
    {
        if (instance == null || instance._bc == null)
        {
            Debug.LogError("BrainCloud instance is not initialized.");
            return;
        }


        string json = $"{{ \"{key}\": {SerializeValue(val)} }}";


        instance._bc.PlayerStateService.UpdateAttributes(
            json,
            false,
            (resp, cbObject) =>
            {
                Debug.Log($"Updated {key} successfully.");
            },
            (status, reason, error, cbObject) =>
            {
                Debug.LogError($"Failed to update {key}: {error}");
            }
        );
    }


    public static void LoadUserProperties(Action<PlayerPropertiesResponse.PlayerProperties> onLoaded)
    {
        instance._bc.PlayerStateService.GetAttributes(
            (jsonResponse, cbObject) =>
            {
                try
                {
                    // Deserialize the JSON into PlayerPropertiesResponse
                    PlayerPropertiesResponse response = JsonUtility.FromJson<PlayerPropertiesResponse>(jsonResponse);

                    if (response != null && response.data != null && response.data.attributes != null)
                    {
                        onLoaded?.Invoke(response.data.attributes);
                    }
                    else
                    {
                        Debug.LogWarning("No player properties found in response.");
                        onLoaded?.Invoke(null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to parse player properties: " + e.Message);
                    onLoaded?.Invoke(null);
                }
            },
            (status, reason, jsonError, cbObject) =>
            {
                Debug.LogError("Failed to load player properties: " + jsonError);
                onLoaded?.Invoke(null);
            }
        );
    }  

    public void FindLobby()
    {
        var algo = new Dictionary<string, object>
        {
            { "strategy", "ranged-absolute" },  // pick one of: "ranged-percent", "ranged-absolute", "compound"
            { "alignment", "center" },
            { "ranges", new List<object> { 100,500,1000 } }
        };


        var extraJson = new Dictionary<string, object>()
        {
            { "attributes", UserManager.GetAttributesDict() }
        };

        _bc.LobbyService.FindOrCreateLobby(
            "Default",      // category of match rules
            in_rating: 0,
            in_maxSteps: 5,
            in_algo: algo,          // required
            in_filterJson: new Dictionary<string, object>(),    // required
            in_isReady: true,
            in_extraJson: extraJson,     // required
            in_teamCode: "",
            in_settings: new Dictionary<string, object>(),      // required
            in_otherUserCxIds: null,
            (response, cbObj) =>
            {
                Debug.Log("[LOBBY CREATED] " + response);

                var msg = JsonReader.Deserialize<Dictionary<string, object>>(response);
                if (msg == null || !msg.ContainsKey("data"))
                    return;

                var data = msg["data"] as Dictionary<string, object>;
                if(data.ContainsKey("lobbyId")) lobbyId = data["lobbyId"].ToString();
            },
            (status, code, error, cbObj) =>
            {
                Debug.LogError($"[LOBBY CREATE FAILED] status={status}, code={code}, error={error}");
            }
        );

    }

    private void LoginSuccessCallback(string jsonResponse, object cbObject)
    {
        Debug.Log(jsonResponse);
        PlayerDataResponse = JsonUtility.FromJson<PlayerDataResponse>(jsonResponse);
        LoginEvent?.Invoke(JsonUtility.FromJson<PlayerDataResponse>(jsonResponse));
        //{"data":{"abTestingId":30,"lastLogin":1763666541217,"server_time":1763666541265,"refundCount":0,"logouts":0,"timeZoneOffset":-6,"experiencePoints":0,"maxBundleMsgs":10,"createdAt":1763659039085,"parentProfileId":null,"emailAddress":"tester@adarcade.io","experienceLevel":0,"countryCode":"US","vcClaimed":0,"currency":{},"id":"ebc4c49f-ece4-4c41-a707-e209a6bb530f","compressIfLarger":51200,"amountSpent":0,"retention":{"d00":true},"previousLogin":1763660697598,"playerName":"","pictureUrl":null,"incoming_events":[],"failedRedemptionsTotal":0,"sessionId":"csi1hf6hns839ci8ksje9u6tmo","languageCode":"en","vcPurchased":0,"isTester":false,"summaryFriendData":null,"loginCount":7,"emailVerified":true,"xpCapped":false,"profileId":"ebc4c49f-ece4-4c41-a707-e209a6bb530f","newUser":"false","allTimeSecs":0,"playerSessionExpiry":1200,"sent_events":[],"maxKillCount":11,"rewards":{"rewardDetails":{},"currency":{},"rewards":{}},"statistics":{}},"status":200}
    }

    private void LoginFailureCallback(int status, int reasonCode, string jsonError, object cbObject)
    {
        Debug.LogError($"FindPlayers FAILED: status={status} code={reasonCode} error={jsonError}");
    }

    private void MatchMakingSuccessCallback(string jsonResponse, object cbObject)
    {
        Debug.Log(jsonResponse);
    }

    private void MatchMakingFailureCallback(int status, int reasonCode, string jsonError, object cbObject)
    {

    }

    private void FindPlayersSuccessCallback(string jsonResponse, object cbObject)
    {
        Debug.Log(jsonResponse);
    }

    private void FindPlayersFailureCallback(int status, int reasonCode, string jsonError, object cbObject)
    {

    }

    private static string SerializeValue(object val)
    {
        if (val is string)
        {
            return $"\"{val}\"";
        }
        else if (val is bool)
        {
            return ((bool)val) ? "true" : "false";
        }
        else
        {
            return val.ToString();
        }
    }


}