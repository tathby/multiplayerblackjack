using System.Collections.Generic;

public static class BlackjackPhotonSerializer
{
    private const string PlayerIdKey = "playerId";
    private const string BetKey = "bet";
    private const string ActionKey = "action";
    private const string PhaseKey = "phase";
    private const string PlayersKey = "players";
    private const string DealerKey = "dealer";
    private const string DealerHiddenKey = "dealerHidden";
    private const string ActiveIndexKey = "activeIndex";
    private const string ChipsKey = "chips";
    private const string HandKey = "hand";

    public static Dictionary<string, object> SerializeBet(PlayerBetMessage message)
    {
        return new Dictionary<string, object>
        {
            { PlayerIdKey, message.PlayerId },
            { BetKey, message.Bet }
        };
    }

    public static PlayerBetMessage DeserializeBet(Dictionary<string, object> data)
    {
        return new PlayerBetMessage(data[PlayerIdKey].ToString(), (int)data[BetKey]);
    }

    public static Dictionary<string, object> SerializeAction(PlayerActionMessage message)
    {
        return new Dictionary<string, object>
        {
            { PlayerIdKey, message.PlayerId },
            { ActionKey, (int)message.Action }
        };
    }

    public static PlayerActionMessage DeserializeAction(Dictionary<string, object> data)
    {
        return new PlayerActionMessage(data[PlayerIdKey].ToString(), (PlayerAction)(int)data[ActionKey]);
    }

    public static Dictionary<string, object> SerializeJoin(PlayerJoinMessage message)
    {
        return new Dictionary<string, object>
        {
            { PlayerIdKey, message.PlayerId }
        };
    }

    public static PlayerJoinMessage DeserializeJoin(Dictionary<string, object> data)
    {
        return new PlayerJoinMessage(data[PlayerIdKey].ToString());
    }

    public static Dictionary<string, object> SerializeState(BlackjackGameStateData data)
    {
        object[] players = new object[data.Players.Count];
        for (int i = 0; i < data.Players.Count; i++)
        {
            PlayerSeatState player = data.Players[i];
            players[i] = new Dictionary<string, object>
            {
                { PlayerIdKey, player.PlayerId },
                { ChipsKey, player.Chips },
                { BetKey, player.Bet },
                { HandKey, SerializeHand(player.Hand) }
            };
        }

        return new Dictionary<string, object>
        {
            { PhaseKey, (int)data.Phase },
            { PlayersKey, players },
            { DealerKey, SerializeHand(data.DealerHand) },
            { DealerHiddenKey, data.DealerHoleCardHidden },
            { ActiveIndexKey, data.ActivePlayerIndex }
        };
    }

    public static BlackjackGameStateData DeserializeState(Dictionary<string, object> data)
    {
        BlackjackGameStateData state = new BlackjackGameStateData
        {
            Phase = (GamePhase)(int)data[PhaseKey],
            DealerHand = DeserializeHand((byte[])data[DealerKey]),
            DealerHoleCardHidden = (bool)data[DealerHiddenKey],
            ActivePlayerIndex = (int)data[ActiveIndexKey]
        };

        object[] players = (object[])data[PlayersKey];
        for (int i = 0; i < players.Length; i++)
        {
            Dictionary<string, object> playerData = (Dictionary<string, object>)players[i];
            PlayerSeatState player = new PlayerSeatState
            {
                PlayerId = playerData[PlayerIdKey].ToString(),
                Chips = (int)playerData[ChipsKey],
                Bet = (int)playerData[BetKey]
            };
            player.Hand = DeserializeHand((byte[])playerData[HandKey]);
            player.UpdateDerivedState();
            state.Players.Add(player);
        }

        return state;
    }

    private static byte[] SerializeHand(Hand hand)
    {
        byte[] encoded = new byte[hand.Count];
        for (int i = 0; i < hand.Count; i++)
        {
            encoded[i] = CardCodec.Encode(hand.Cards[i]);
        }
        return encoded;
    }

    private static Hand DeserializeHand(byte[] data)
    {
        Hand hand = new Hand();
        for (int i = 0; i < data.Length; i++)
        {
            hand.AddCard(CardCodec.Decode(data[i]));
        }
        return hand;
    }
}
