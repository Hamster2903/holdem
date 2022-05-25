using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerClassScript : MonoBehaviour
{
    
    public int numOfChips = 1000;
    public int numOfChipsInPot = 0;
    public bool hasFolded = false;
    public bool hasRaised = false;
    public bool hasCalled = false;
    public bool isActive = false;
    public int mostRecentBet;
    public string playerName = "";
    public int valueOfCardsInHand = 0;
    public bool isLittleBlind;
    public bool isBigBlind;
    public bool isDealer;
    public List<GameObject> cards;

    public playerClassScript(string newName, bool newLittleBlind, bool newBigBlind)
    {
        
        this.numOfChips = 1000;
        this.numOfChipsInPot = 0;
        this.hasFolded = false;
        this.playerName = newName;
        this.valueOfCardsInHand = 0;
        this.hasCalled = false;
        this.hasRaised = false;
        this.isLittleBlind = newLittleBlind;
        this.isBigBlind = newBigBlind;
        this.cards = new List<GameObject>();
        this.isActive = false;
        this.isDealer = false;
        this.mostRecentBet = 0;
    }


   
}
