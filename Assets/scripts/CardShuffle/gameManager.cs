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
    public int potValue;
    public int callValue;
    public int raiseValue;
    public int round;
    public int activePlayer;
    public bool allFolded = false;
    public bool potOver = false;
    public List<GameObject> players;
    public GameObject playerPrefab;
    public GameObject cardGroups;
    public List<GameObject> flopList;
    public GameObject flopGrid;
    public List<GameObject> playerPositions;
    public bool isPlayerName;
    //make bool checking if playerName  so each player can see what their cards is but not what others are
    void Start()
    {
        round = 0;
        deckScript.Generate();
        deckScript.Shuffle();
        //run pre-flop round
        DealToFlop1();
        GeneratePlayers(3);
        GeneratePlayerObjects();
        DealToHands();
    }
   
    public void gameState()
    {
       
        
        if(round == 1)
        {
            DealToFlop1();
        }
        if(round == 2)
        {
            DealToTurn();
        }
        if(round == 3)
        {
            DealToRiver();
        }
       
        
    }
    public void GeneratePlayers(int numPlayers)
    {
        players.Clear();
        for (int i = 0; i < numPlayers; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            players.Add(newPlayer);

        }
        print(numPlayers);
    }
    public void DealToHands()
    {
        for (int i = 0; i < players.Count; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
                deckScript.cards.Remove(cardToMove); //removes card from list
                players[i].GetComponent<playerClassScript>().cards.Add(cardToMove); //adds removed card to player list for each player.
                cardToMove.transform.SetParent(players[i].transform);//instantiates cards from list to player grid
                print(cardToMove);
            }
            
        }
    }
    
    public void Rotation()
    {
        //sets first player in position to active, sequences through each player and sets the next one as false, use this code on the buttonOnClick functions
        int activePlayerPosition = 0;
        players[activePlayerPosition].
        players[activePlayerPosition % players.Count];
        activePlayerPosition += 1;
        players[activePlayerPosition % players.Count]
        if(activePlayerPosition == 0)
        {
            round += 1;
        }
        //place this code in ech button and then desihgn it so that it does the specific code for each button instead of using this as a function
    }
    public void GeneratePlayerObjects()
    {
        int count = 0;
        //defines player position in objects as its position in the list
        GameObject playerPositionsParent = playerPositions[players.Count - 3];
        foreach (Transform item in playerPositionsParent.transform)//for each position in the list instantiates players
        {
            print("hello");
            players[count].transform.SetParent(item.transform);
            players[count].transform.position = item.position;
            count++;
        }

    }
    public void DealToFlop1()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
            deckScript.cards.Remove(cardToMove); //removes card from list
            flopList.Add(cardToMove);
            cardToMove.transform.SetParent(flopGrid.transform);
        }
    }
    public void DealToTurn()
    {
        GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
        deckScript.cards.Remove(cardToMove); //removes card from list
        flopList.Add(cardToMove);
        cardToMove.transform.SetParent(flopGrid.transform);
    }
    public void DealToRiver()
    {
        GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
        deckScript.cards.Remove(cardToMove); //removes card from list
        flopList.Add(cardToMove);
        cardToMove.transform.SetParent(flopGrid.transform);
    }
    public void RaiseOnClick()
    {
        Rotation();
        
        //remove amount from players class and add raised amount in pot value
       
    }
    public void CallOnCLick()
    {
        Rotation();
        //match previous bet, remove amount from player class and store in pot
    }
    public void FoldOnCLick()
    {
        Rotation();
        //change player.hasFolded to true 
    }
    public void DistributePot()
    {
        //adds pot value to specific players
    }
}
