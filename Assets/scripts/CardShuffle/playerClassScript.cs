using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassScript : MonoBehaviour
{
    
    private int valueOfChipsBet = 0;
    private bool folded = false;
    private string playerName = "";
    private int valueOfCardsInHand = 0;
    private bool isLittleBlind;
    private bool isBigBlind;
    public List<GameObject> cards;

    public PlayerClassScript(string newName, bool newLittleBlind, bool newBigBlind)
    {
        
        this.valueOfChipsBet = 0;
        this.folded = false;
        this.playerName = newName;
        this.valueOfCardsInHand = 0;
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
