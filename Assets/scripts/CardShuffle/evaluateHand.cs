using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class evaluateHand : MonoBehaviour
{
    int royalFlush = 9;
    int straightFlush = 8;
    int fourOfAKind = 7;
    int fullHouse = 6;
    int flush = 5;
    int straight = 4;
    int threeOfAKind = 3;
    int twoPair = 2;
    int onePair = 1;
    int highCard = 0;
    
    //must sort the best 5 card combination so that it can determine straights, 5 increasing cards
    //must sort the best 5 card combination by suit so that it may determine flushes
    //must determine if there are 4 cards of the same number
    //must determine for 3 cards
    //for a two pair, two different pairs
    //one pair, two of the same number

}
