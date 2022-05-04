using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public DeckScript deckScript;
    public Button raiseButton;
    public Button callButton;
    public Button foldButton;
    public int littleBlindBetValue;
    public int bigBlindBetValue;
    public int callValue;
    public int raiseValue;
    public int roundValue;
    void Start()
    {
        deckScript.Generate();
        deckScript.Shuffle();
        deckScript.DealToFlop();
        deckScript.DealToHand();
    }
   
    //pre-flop, little and big blinds bet, moves around table until everyone has acted
    //store and update round value based on community cards
    //deal to turn on the second round, players re-bet/call/fold/raise
    //deal to river on the third round, players re-bet/call/fold/raise
    //showdown, the card hands are evualated and the winner is determined


}
