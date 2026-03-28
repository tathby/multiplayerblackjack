using System;
using System.Collections.Generic;

[Serializable]
public class BlackjackGameStateData
{
    public GamePhase Phase = GamePhase.Betting;
    public List<PlayerSeatState> Players = new List<PlayerSeatState>();
    public Hand DealerHand = new Hand();
    public Deck Deck = new Deck();
    public bool DealerHoleCardHidden = true;
    public int ActivePlayerIndex = 0;
    public int MinBet = 10;
    public int MaxBet = 1000;

    public PlayerSeatState GetPlayer(string playerId)
    {
        return Players.Find(p => p.PlayerId == playerId);
    }

    public PlayerSeatState GetOrCreatePlayer(string playerId, int startingChips)
    {
        PlayerSeatState player = GetPlayer(playerId);
        if (player != null)
        {
            return player;
        }

        player = new PlayerSeatState
        {
            PlayerId = playerId,
            Chips = startingChips
        };
        Players.Add(player);
        return player;
    }

    public void ResetForNewRound()
    {
        DealerHand.Clear();
        DealerHoleCardHidden = true;
        ActivePlayerIndex = 0;
        for (int i = 0; i < Players.Count; i++)
        {
            Players[i].ResetForRound();
        }

        if (Deck.Count < 20)
        {
            Deck.ResetAndShuffle();
        }
    }

    public bool AreAllBetsPlaced()
    {
        if (Players.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].Bet < MinBet)
            {
                return false;
            }
        }
        return true;
    }

    public bool AreAllPlayersDone()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            PlayerSeatState player = Players[i];
            if (!player.HasStood && !player.HasBusted && !player.HasBlackjack)
            {
                return false;
            }
        }
        return true;
    }

    public PlayerSeatState GetActivePlayer()
    {
        if (Players.Count == 0 || ActivePlayerIndex < 0 || ActivePlayerIndex >= Players.Count)
        {
            return null;
        }
        return Players[ActivePlayerIndex];
    }

    public void AdvanceToNextPlayer()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            ActivePlayerIndex = (ActivePlayerIndex + 1) % Players.Count;
            PlayerSeatState player = Players[ActivePlayerIndex];
            if (!player.HasStood && !player.HasBusted && !player.HasBlackjack)
            {
                return;
            }
        }
    }
}
