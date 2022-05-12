using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerClassScript : MonoBehaviour
{
    
    private int valueOfChipsBet = 0;
    private bool hasFolded = false;
    private bool hasRaised = false;
    private bool hasCalled = false;
    private string playerName = "";
    private int valueOfCardsInHand = 0;
    private bool isLittleBlind;
    private bool isBigBlind;
    public List<GameObject> cards;

    public playerClassScript(string newName, bool newLittleBlind, bool newBigBlind)
    {
        
        this.valueOfChipsBet = 0;
        this.hasFolded = false;
        this.playerName = newName;
        this.valueOfCardsInHand = 0;
        this.hasCalled = false;
        this.hasRaised = false;
        this.isLittleBlind = newLittleBlind;
        this.isBigBlind = newBigBlind;
        this.cards = new List<GameObject>();
    }


    /*public bool getFolded()
    {
        return this.folded;
    }
    public int getNumOfCardsInHand()
    {
        return this.numOfCardsInHand;
    }
    public int getValueOfChipsBet()
    {
        return this.valueOfChipsBet;
    }
    public string getPlayerName()
    {
        return this.playerName;
    }
    public int getValueOfCardsinHand()
    {
        return this.valueOfCardsInHand;
        
    }*/
}
