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
    void Start()
    {
        deckScript.Generate();
        deckScript.Shuffle();
        deckScript.DealToFlop();
        deckScript.DealToHand();
    }

    
    void Update()
    {
        
    }
}
