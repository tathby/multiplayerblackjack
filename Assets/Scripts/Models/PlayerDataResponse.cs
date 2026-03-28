using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerDataResponse
{
    public PlayerDataWrapper data;
    public int status;

    [Serializable]
    public class PlayerDataWrapper
    {
        public int abTestingId;
        public long lastLogin;
        public long server_time;
        public int refundCount;
        public int logouts;
        public int timeZoneOffset;
        public int experiencePoints;
        public int maxBundleMsgs;
        public long createdAt;
        public string parentProfileId;
        public string emailAddress;
        public int experienceLevel;
        public string countryCode;
        public int vcClaimed;
        public SerializableDictionary<string, object> currency;
        public string id;
        public int compressIfLarger;
        public int amountSpent;
        public RetentionData retention;
        public long previousLogin;
        public string playerName;
        public string pictureUrl;
        public List<object> incoming_events;
        public int failedRedemptionsTotal;
        public string sessionId;
        public string languageCode;
        public int vcPurchased;
        public bool isTester;
        public string summaryFriendData;
        public int loginCount;
        public bool emailVerified;
        public bool xpCapped;
        public string profileId;
        public string newUser;
        public int allTimeSecs;
        public int playerSessionExpiry;
        public List<object> sent_events;
        public int maxKillCount;
        public RewardsData rewards;
        public SerializableDictionary<string, object> statistics;

        [Serializable]
        public class RetentionData
        {
            public bool d00;
        }

        [Serializable]
        public class RewardsData
        {
            public RewardDetailsData rewardDetails;
            public SerializableDictionary<string, object> currency;
            public SerializableDictionary<string, object> rewards;

            [Serializable]
            public class RewardDetailsData
            {
                // Populate if you know inner structure
            }
        }
    }
}

/// <summary>
/// Helper class for serializing dictionaries with JsonUtility
/// </summary>
[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var kvp in _dict)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        _dict = new Dictionary<TKey, TValue>();
        for (int i = 0; i < Math.Min(keys.Count, values.Count); i++)
        {
            _dict[keys[i]] = values[i];
        }
    }

    public TValue this[TKey key]
    {
        get => _dict[key];
        set => _dict[key] = value;
    }

    public void Add(TKey key, TValue value) => _dict.Add(key, value);
    public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);
    public Dictionary<TKey, TValue> ToDictionary() => _dict;
}
