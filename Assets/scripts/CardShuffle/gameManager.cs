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
    public Text potValueText;
    public int littleBlindBetValue = 5;
    public int bigBlindBetValue = 10;
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
        GeneratePlayers(3,1);
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

    //make little and big blinds bet automatically, activePlayerPosition +=1 from the big blind player position
    
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
    public void GeneratePlayers(int numPlayers, int gameNum)
    {
        
        players.Clear();
        for (int i = 0; i < numPlayers; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            players.Add(newPlayer);
            newPlayer.gameObject.GetComponent<Image>().enabled = false;

        }
        playerClassScript dealerPlayer = players[(-1 + gameNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript littleBlindPlayer = players[(0 + gameNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript bigBlindPlayer = players[(1 + gameNum) % numPlayers].GetComponent<playerClassScript>();
        dealerPlayer.isDealer = true;//set position 0 to dealer button, position 1 to little blind, position 2 to big blind
        littleBlindPlayer.isLittleBlind = true;
        bigBlindPlayer.isBigBlind = true;
        littleBlindPlayer.mostRecentBet =5;
        bigBlindPlayer.mostRecentBet = 10;
        mostRecentBet = bigBlindPlayer.mostRecentBet;
        activePlayerPosition = (2 + gameNum) % numPlayers;
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;

        DebugPrint("active player position",activePlayerPosition);
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
        //remove numOfChips, add to numOfChipsInPot and add to int potValue;
        //keep track of most recent bet so that the call function may use it
        raiseValue = mostRecentBet;
        mostRecentBet = currentPlayer.mostRecentBet;
        currentPlayer.numOfChips -= raiseValue;
        currentPlayer.numOfChipsInPot +=currentPlayer.mostRecentBet;
        
        activePlayerPosition += 1;// Go to next player
        // where are we at, betting still, or next round?
        //checkBettingStatus?
        checkBettingStatus();
        DebugPrint("playerPos", activePlayerPosition);
    }


    public void CallOnClick()
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("call button working");
        //remove chips from numOfChips and add to numOfChipsInPot and potValue
        //most recent bet = amount of chips to be added to pot and numOfChipsInPot
        //set the amount of chips to be added to pot and numOfChipsInPot to the new mostRecentBet
        currentPlayer.mostRecentBet = mostRecentBet;
        mostRecentBet = currentPlayer.mostRecentBet;
        currentPlayer.numOfChips -= currentPlayer.mostRecentBet;
        currentPlayer.numOfChipsInPot +=currentPlayer.mostRecentBet;
        activePlayerPosition += 1;//increases active player by one position
        checkBettingStatus();

    }
    public void FoldOnClick()
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("fold button working");
        currentPlayer.gameObject.SetActive(false);
        activePlayerPosition += 1;
        checkBettingStatus();
    }
    public void DistributePot()//will be run when players cards are evaluated or everyone folds
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        currentPlayer.numOfChips += potValue;//sets potValue to 0, sets numOfChipsInPot and adds to numOfChips on playerClassScript of player who won
        //gameNum++;
    }

    //check betting status function
    public void checkBettingStatus()
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        //chekd current bet, loop for ech player checking if currentPlayer.mostRecentBet = mostRecentBet
        //if true then increment to next round
        for (int i = 0; i < players.Count; i++)
        {
            if (mostRecentBet == currentPlayer.mostRecentBet)
            {
                round++;
                potValue += currentPlayer.numOfChipsInPot;
                potValueText.text = Convert.ToString(potValue);
            }
            else
            {
                //keep loopijg, null?
            }
        }
    }
}
