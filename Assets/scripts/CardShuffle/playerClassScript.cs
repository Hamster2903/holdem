using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassScript : MonoBehaviour
{
    private int numOfCardsInHand = 0;
    private int valueOfChipsBet = 0;
    private bool folded = false;
    private string playerName = "";
    private int valueOfCardsInHand = 0;
    

    public PlayerClassScript(int newNofCards, int newChipValue, bool newFolded, string newName, int newCardValue)
    {
        this.numOfCardsInHand = newNofCards;
        this.valueOfChipsBet = newChipValue;
        this.folded = newFolded;
        this.playerName = newName;
        this.valueOfCardsInHand = newCardValue;
        
    }

    public bool getFolded()
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
        
    }
}
