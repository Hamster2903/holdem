using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassScript : MonoBehaviour
{
    
    private int valueOfChipsBet = 0;
    private int individualValueOfBet = 0;
    private bool folded = false;
    private string playerName = "";
    private int valueOfCardsInHand = 0;
    private bool isLittleBlind;
    private bool isBigBlind;
    public List<GameObject> cards;

    public PlayerClassScript(int newChipValue, int newIndividualBet,bool newFolded, string newName, int newCardValue, bool newLittleBlind, bool newBigBlind)
    {
        
        this.valueOfChipsBet = newChipValue;
        this.individualValueOfBet = newIndividualBet;
        this.folded = newFolded;
        this.playerName = newName;
        this.valueOfCardsInHand = newCardValue;
        this.isLittleBlind = newLittleBlind;
        this.isBigBlind = newBigBlind;
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
