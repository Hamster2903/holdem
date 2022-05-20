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
    public int minimumBetValue = 10;
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
    public bool debug = false;
    //make bool checking if playerName  so each player can see what their cards is but not what others are
    
    public void DebugPrint(string prefix, object message)
    {
        if(debug)
        {
            print(prefix + ": " + message);
        }
    }
    void Start()
    {
        round = 0;
        deckScript.Generate();
        deckScript.Shuffle();
        GeneratePlayers(3);
        GeneratePlayerObjects();
        DealToHands();
        GameLoop();
    }
    //make fucntion that sets 3 players as either big blind little blind or dealer position
    //little blind bets half the minimum bet (5) big blind must bet 10 in this instance
    // so if player is little blind bet 5 if player is big blind must raise to 10 then set this as most recent bet
    //if players are to raise a bet it must be atleast double the previous bet
    //once all players have bet it comes back around to the small blind, they can either match the big blinds bet, raise or fold
    //the last player to act in the rotation will be the big blind, they can raise the bet, if they do everyone must act again until back to the big blind.
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
        players[0].GetComponent<playerClassScript>().isDealer = true;//set position 0 to dealer button, position 1 to little blind, position 2 to big blind
        players[1].GetComponent<playerClassScript>().isLittleBlind = true;
        players[2].GetComponent<playerClassScript>().isBigBlind = true;
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
        
        GameObject playerPositionsParent = playerPositions[players.Count - 3];//defines player position in objects as its position in the list
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
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("raise button working");
        string raiseInputText = raiseInput.text;//sets the string value recorded in the input text to an integer which will be used to represent features on the player script
        raiseValue = int.Parse(raiseInputText) +mostRecentBet*2;//raise value is equal to the players input + 2 times the most recent bet because to raise the bet it must be atleast two times the previous bet
        currentPlayer.hasRaised = true;
        
        //remove numOfChips, add to numOfChipsInPot and add to int potValue;
        //keep track of most recent bet so that the call function may use it
        raiseValue = mostRecentBet;
        currentPlayer.numOfChips -= raiseValue;
        currentPlayer.numOfChipsInPot +=raiseValue;
        potValue +=currentPlayer.numOfChipsInPot;
        print(activePlayerPosition % players.Count);
        DebugPrint("Pot Value",potValue);
        activePlayerPosition += 1;// Go to next player
        // where are we at, betting still, or next round?
        //checkBettingStatus?
    }

    //check betting status function, returns bool
    public void checkBettingStatus()
    {
        //checks if bigblind has raised
    }

    public void CallOnClick()
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("call button working");
        //sets the current player to hasCalled = true
        currentPlayer.hasCalled = true;
        //activePlayerPosition += 1; //increases active player by one position
        currentPlayer.hasCalled = false; //sets new active player as hasCalled = false
        if (activePlayerPosition % players.Count == 0) //if the activeplayer rotates back to its original position increment the round by one
        {
            round += 1;
        }
        else if(currentPlayer.hasCalled == true) // if the active player hasCalled runs the code which completes the  call action
        {
            //remove chips from numOfChips and add to numOfChipsInPot and potValue
            //most recent bet = amount of chips to be added to pot and numOfChipsInPot
            //set the amount of chips to be added to pot and numOfChipsInPot to the new mostRecentBet
            currentPlayer.numOfChips -= mostRecentBet;
            currentPlayer.numOfChipsInPot +=mostRecentBet;
            potValue += mostRecentBet;
        }
        print(activePlayerPosition % players.Count);
        print(mostRecentBet);
    }
    public void FoldOnClick()
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("fold button working");
        currentPlayer.hasFolded = true;
        //activePlayerPosition += 1;
        currentPlayer.hasFolded = false;
        if (activePlayerPosition % players.Count==0)
        {
            round += 1;
        }
        else if(currentPlayer.hasFolded == true)
        {
            players[activePlayerPosition].gameObject.SetActive(false);
            potValue += currentPlayer.numOfChipsInPot;//remove numOfChipsInPot and add to int potValue;
        }
        print(activePlayerPosition % players.Count);
    }
    public void DistributePot()//will be run when players cards are evaluated or everyone folds
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        currentPlayer.numOfChips += potValue;//sets potValue to 0, sets numOfChipsInPot and adds to numOfChips on playerClassScript of player who won
        //rotate little, big blinds and dealer +1;
    }
}
