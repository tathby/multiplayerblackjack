using System;
using System.Collections.Generic;

[Serializable]
public class PlayerPropertiesResponse
{
    public PlayerPropertiesWrapper data;
    public int status;

    //maybe combine this into the playerdataresponse

    [Serializable]
    public class PlayerPropertiesWrapper
    {
        public PlayerProperties attributes;
    }

    [Serializable]
    public class PlayerProperties
    {
        public string colorHex;

        public Dictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>
            {
                { "colorHex", colorHex }
            };
            return dict;
        }
    }

}
