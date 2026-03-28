using System;

public static class CardCodec
{
    public static byte Encode(Card card)
    {
        int suit = (int)card.Suit;
        int rank = RankToIndex(card.Rank);
        return (byte)(suit * 13 + rank);
    }

    public static Card Decode(byte value)
    {
        int suit = value / 13;
        int rank = value % 13;
        return new Card((CardSuit)suit, IndexToRank(rank));
    }

    private static int RankToIndex(CardRank rank)
    {
        return rank switch
        {
            CardRank.Two => 0,
            CardRank.Three => 1,
            CardRank.Four => 2,
            CardRank.Five => 3,
            CardRank.Six => 4,
            CardRank.Seven => 5,
            CardRank.Eight => 6,
            CardRank.Nine => 7,
            CardRank.Ten => 8,
            CardRank.Jack => 9,
            CardRank.Queen => 10,
            CardRank.King => 11,
            CardRank.Ace => 12,
            _ => 0
        };
    }

    private static CardRank IndexToRank(int index)
    {
        return index switch
        {
            0 => CardRank.Two,
            1 => CardRank.Three,
            2 => CardRank.Four,
            3 => CardRank.Five,
            4 => CardRank.Six,
            5 => CardRank.Seven,
            6 => CardRank.Eight,
            7 => CardRank.Nine,
            8 => CardRank.Ten,
            9 => CardRank.Jack,
            10 => CardRank.Queen,
            11 => CardRank.King,
            12 => CardRank.Ace,
            _ => CardRank.Two
        };
    }
}
