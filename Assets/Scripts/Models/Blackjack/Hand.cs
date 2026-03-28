using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Hand
{
    [SerializeField] private List<Card> cards = new List<Card>();

    public IReadOnlyList<Card> Cards => cards;
    public int Count => cards.Count;

    public void Clear() => cards.Clear();

    public void AddCard(Card card) => cards.Add(card);

    public int GetBestValue()
    {
        int total = 0;
        int aceCount = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            CardRank rank = cards[i].Rank;
            if (rank == CardRank.Ace)
            {
                aceCount++;
                total += 1;
            }
            else if (rank >= CardRank.Jack && rank <= CardRank.King)
            {
                total += 10;
            }
            else
            {
                total += (int)rank;
            }
        }

        for (int i = 0; i < aceCount; i++)
        {
            if (total + 10 <= 21)
            {
                total += 10;
            }
        }

        return total;
    }

    public bool IsBlackjack() => cards.Count == 2 && GetBestValue() == 21;
    public bool IsBust() => GetBestValue() > 21;
}
