using UnityEngine;

[CreateAssetMenu(menuName = "Blackjack/Dealer AI")]
public class DealerAI : ScriptableObject
{
    [SerializeField] private int hitThreshold = 16;

    public bool ShouldHit(Hand dealerHand)
    {
        if (dealerHand == null)
        {
            return false;
        }

        return dealerHand.GetBestValue() <= hitThreshold;
    }
}
