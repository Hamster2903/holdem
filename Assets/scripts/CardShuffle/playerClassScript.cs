using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassScript : MonoBehaviour
{
    private int numOfCardsInHand = 0;
    private int valueOfChipsBet = 0;
    private bool folded = false;
    private string playerName = "";
    private string valueOfCardsInHand = "";
    private int numOfRoundsIn = 0;

    public PlayerClassScript(int newNofCards, int newChipValue, bool newFolded, string newName, string newCardValue, int newRoundNum)
    {
        this.numOfCardsInHand = newNofCards;
        this.valueOfChipsBet = newChipValue;
        this.folded = newFolded;
        this.playerName = newName;
        this.valueOfCardsInHand = newCardValue;
        this.numOfRoundsIn = newRoundNum;
    }

    public bool getFolded()
    {
        return this.folded;
    }
}
