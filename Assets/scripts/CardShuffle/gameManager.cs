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
    //gameLoop (while)
    public void gameState()
    {
       
        //sets first player in position to active, sequences through each player and sets the next one as false, use this code on the buttonOnClick functions
        int activePlayerPosition = 0;
        players[activePlayerPosition].isActive = true;
        players[activePlayerPosition % players.Count].isActive = false;
        activePlayerPosition += 1;
        players[activePlayerPosition % players.Count].isActive = true;
        
        //set 1 player to active in the list of all players
        //this player is allowed to act
        //once this player has acted increment active player by 1, set previously active player to inactive
        //once all players in the list have acted, set active player back to 1 and increment round by 1
        //run round specific functions, i.e. round 1 will have the 

        //
       
        
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
    public void DealToFlop2()
    {
        GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
        deckScript.cards.Remove(cardToMove); //removes card from list
        flopList.Add(cardToMove);
        cardToMove.transform.SetParent(flopGrid.transform);
    }
    public void RaiseOnClick()
    {
        
        int activePlayerPosition = 0;
        players[activePlayerPosition].isActive = true;
        players[activePlayerPosition % players.Count].isActive = false;
        activePlayerPosition += 1;
        players[activePlayerPosition % players.Count].isActive = true;
    }
    public void CallOnCLick()
    {
        int activePlayerPosition = 0;
        players[activePlayerPosition].isActive = true;
        players[activePlayerPosition % players.Count].isActive = false;
        activePlayerPosition += 1;
        players[activePlayerPosition % players.Count].isActive = true;
    }
    public void FoldOnCLick()
    {
        
        int activePlayerPosition = 0;
        players[activePlayerPosition].isActive = true;
        players[activePlayerPosition % players.Count].isActive = false;
        activePlayerPosition += 1;
        players[activePlayerPosition % players.Count].isActive = true;
    }
}
