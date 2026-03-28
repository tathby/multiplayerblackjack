using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Deck
{
    [SerializeField] private List<Card> cards = new List<Card>();

    public int Count => cards.Count;
    public IReadOnlyList<Card> Cards => cards;

    public void ResetAndShuffle()
    {
        cards.Clear();
        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
            {
                cards.Add(new Card(suit, rank));
            }
        }
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    public Card Deal()
    {
        if (cards.Count == 0)
        {
            ResetAndShuffle();
        }

        Card card = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        return card;
    }
}
