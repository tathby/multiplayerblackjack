using System;
using UnityEngine;

[Serializable]
public class PlayerSeatState
{
    public string PlayerId;
    public int Chips;
    public int Bet;
    public Hand Hand = new Hand();
    public bool HasStood;
    public bool HasBusted;
    public bool HasBlackjack;
    public bool HasDoubled;

    public void ResetForRound()
    {
        Bet = 0;
        HasStood = false;
        HasBusted = false;
        HasBlackjack = false;
        HasDoubled = false;
        Hand.Clear();
    }

    public void UpdateDerivedState()
    {
        HasBlackjack = Hand.IsBlackjack();
        HasBusted = Hand.IsBust();
    }
}
