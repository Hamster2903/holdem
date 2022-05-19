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
    public InputField raiseInput;
    public int littleBlindBetValue;
    public int bigBlindBetValue;
    public int potValue = 0;
    public int callValue;
    public int raiseValue;
    public int round;
    public int mostRecentBet;
    public int activePlayerPosition = 0;
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
        GeneratePlayers(3);
        GeneratePlayerObjects();
        DealToHands();
        GameLoop();
    }
    //make fucntion that sets 3 players as either big blind little blind or dealer position
    //little blind may bet a small amount of chips either set or ranging from 5 to 20, big blind must bet 2x that
    public void GameLoop()
    {
       
        
        if(round == 1)
        {
            DealToFlop();
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
    public void DealToFlop()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
            deckScript.cards.Remove(cardToMove); //removes card from list
            flopList.Add(cardToMove);//adds removed card to lsit
            cardToMove.transform.SetParent(flopGrid.transform);//puts card in game object grid
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
        print("raise button working");
        //sets the string value recorded in the input text to an integer which will be used to represent features on the player script
        string raiseInputText = raiseInput.text;
        raiseValue = int.Parse(raiseInputText);
        players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().hasRaised = true;
        activePlayerPosition += 1;
        players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().hasRaised = false;
        if (activePlayerPosition == 0)
        {
            round += 1;
        }
        else if(players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().hasRaised == true)
        {
            //remove numOfChips, add to numOfChipsInPot and add to int potValue;
            //keep track of most recent bet so that the call function may use it
            raiseValue = mostRecentBet;
            players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChips =(players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChips -= raiseValue);
            players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChipsInPot = (players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChipsInPot += raiseValue);
            potValue +=players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChipsInPot;
        }
        print(players[activePlayerPosition % players.Count]);
    }
    public void CallOnClick()
    {
        print("call button working");
        //sets the current player to hasCalled = true
        players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().hasCalled = true;
        activePlayerPosition += 1; //increases active player by one position
        players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().hasCalled = false; //sets new active player as hasCalled = false
        if (activePlayerPosition == 0) //if the activeplayer rotates back to its original position increment the round by one
        {
            round += 1;
        }
        else if(players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().hasCalled == true) // if the active player hasCalled runs the code which completes the  call action
        {
            //remove chips from numOfChips and add to numOfChipsInPot and potValue
            //most recent bet = amount of chips to be added to pot and numOfChipsInPot
            //set the amount of chips to be added to pot and numOfChipsInPot to the new mostRecentBet
            players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChips = (players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChips -= mostRecentBet);
            players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChipsInPot +=(players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChipsInPot += mostRecentBet);
            potValue += mostRecentBet;
        }
        print(players[activePlayerPosition % players.Count]);
    }
    public void FoldOnClick()
    {
        print("fold button working");
        players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().hasFolded = true;
        activePlayerPosition += 1;
        players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().hasFolded = false;
        if (activePlayerPosition == 0)
        {
            round += 1;
        }
        else if(players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().hasFolded == true)
        {
            players[activePlayerPosition].gameObject.SetActive(false);
            //remove numOfChipsInPot and add to int potValue;
            potValue += players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChipsInPot;
        }
        print(players[activePlayerPosition % players.Count]);
    }
    public void DistributePot()//will be run when players cards are evaluated or everyone folds
    {
        //sets potValue to 0, sets numOfChipsInPot and adds to numOfChips on playerClassScript of player who won
        players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChips =(players[activePlayerPosition % players.Count].GetComponent<playerClassScript>().numOfChips += potValue);
    }
}
