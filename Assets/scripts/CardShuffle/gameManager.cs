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
    public int round = 0;
    public int mostRecentBet;
    public int numberPlayers;
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
        deckScript.Generate();
        deckScript.Shuffle();
        GeneratePlayers(3,1);
        GeneratePlayerObjects();
        DealToHands();
        GameLoop();
    }
    //little blind bets half the minimum bet (5) big blind must bet 10 in this instance
    // so if player is little blind bet 5 if player is big blind must raise to 10 then set this as most recent bet
    //if players are to raise a bet it must be atleast double the previous bet
    //once all players have bet it comes back around to the small blind, they can either match the big blinds bet, raise or fold
    //the last player to act in the rotation will be the big blind, they can raise the bet, if they do everyone must act again until back to the big blind
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
        numberPlayers = numPlayers;
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
        potValue += littleBlindPlayer.mostRecentBet;
        bigBlindPlayer.mostRecentBet = 10;
        potValue += bigBlindPlayer.mostRecentBet;
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
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;//this is running off of the wrong number , it should be using the modulo thing but it cannot access this as that is apart of generate players
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("raise button working");
        string raiseInputText = raiseInput.text;//sets the string value recorded in the input text to an integer which will be used to represent features on the player script
        raiseValue = int.Parse(raiseInputText) +mostRecentBet*2;//raise value is equal to the players input + 2 times the most recent bet because to raise the bet it must be atleast two times the previous bet
        mostRecentBet=raiseValue;
        DebugPrint("most recent bet is", mostRecentBet); //checking to see what the mostRecent bet is and if it is correctly storing the bet value so it can be distributed to the potValue correctly
        currentPlayer.mostRecentBet = mostRecentBet;//the players mostRecentBet is set equal to the global mostRecentBet so that the players mostRecentBet is updated
        currentPlayer.numOfChips -= mostRecentBet;//the players total number of chips has the raise value subtracted from it so that the players cumulative number of chips is updated
        currentPlayer.numOfChipsInPot +=currentPlayer.mostRecentBet;//the players total numberOfChipsInPot has the playersMostRecentBet added to it so that the player cumulative bet in the pot is updated
        activePlayerPosition = (activePlayerPosition+1) % (numberPlayers);// Go to next player
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;//this is running off of the wrong number , it should be using the modulo thing but it cannot access this as that is apart of generate players
        checkBettingStatus();
    }


    public void CallOnClick()
    {
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;//this is running off of the wrong number , it should be using the modulo thing but it cannot access this as that is apart of generate players
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("call button working");
        currentPlayer.mostRecentBet = mostRecentBet;//the players mostRecentBet is set equal to the previous global mostRecentBet
        currentPlayer.numOfChips -= currentPlayer.mostRecentBet;//the players cumulative amount of chips has their mostRecentBet subtracted from it
        currentPlayer.numOfChipsInPot +=currentPlayer.mostRecentBet;//the players cumulative bet in the current pot has their mostRecentBet added to it
        DebugPrint("most recent bet", currentPlayer.mostRecentBet);
        activePlayerPosition = (activePlayerPosition + 1) % (numberPlayers);//increases active player by one position
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;//this is running off of the wrong number , it should be using the modulo thing but it cannot access this as that is apart of generate players
        checkBettingStatus();

    }
    public void FoldOnClick()
    {
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;//this is running off of the wrong number , it should be using the modulo thing but it cannot access this as that is apart of generate players
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("fold button working");
        currentPlayer.gameObject.SetActive(false);
        activePlayerPosition = (activePlayerPosition + 1) % (numberPlayers);
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;//this is running off of the wrong number , it should be using the modulo thing but it cannot access this as that is apart of generate players
        checkBettingStatus();
    }
    public void DistributePot()//will be run when players cards are evaluated or everyone folds
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        currentPlayer.numOfChips += potValue;//sets potValue to 0, sets numOfChipsInPot and adds to numOfChips on playerClassScript of player who won
        //gameNum++;

    }

    public void checkBettingStatus()
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        //playerClassScript bigBlindPlayer = players[(1 + gameNum) % numPlayers].GetComponent<playerClassScript>();//how would i solve this problem if the gameNum and numPlayers is defined by the generatePlayer Function, this applies also to the image problem as the activePlayer integer is not the same as it is when defined in genrate players.
        //chekd current bet, loop for ech player checking if currentPlayer.mostRecentBet = mostRecentBet
        //if true then increment to next round
        for (int i = 0; i < players.Count; i++)
        {
            if (currentPlayer.mostRecentBet==mostRecentBet)
            {
                round+=1;
                potValue += currentPlayer.numOfChipsInPot;
                potValueText.text = Convert.ToString(potValue);
                DebugPrint("round value is ", round); //checking what the round is, as the flop was not being dealt.
            }
            else
            {
                //keep loopijg, null?
            }
        }
    }
}
//logic error with betting status function as with the call action the currentPlayer mostrecentbet automically fulfills the if statement thus incrementing the round and increasing the potValue prematurely