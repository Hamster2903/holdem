using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
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
    public int gameNumber;
    public int totalChipsInPot;
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
        GeneratePlayers(5,1);
        GeneratePlayerObjects();
        DealToHands();
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
        gameNumber = gameNum;
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
        potValueText.text = Convert.ToString(potValue);
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
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("raise button working");
        string raiseInputText = raiseInput.text;//sets the string value recorded in the input text to an integer which will be used to represent features on the player script
        raiseValue = int.Parse(raiseInputText) +mostRecentBet*2;//raise value is equal to the players input + 2 times the most recent bet because to raise the bet it must be atleast two times the previous bet
        mostRecentBet = raiseValue;
        DebugPrint("most recent bet is", mostRecentBet); //checking to see what the mostRecent bet is and if it is correctly storing the bet value so it can be distributed to the potValue correctly
        currentPlayer.mostRecentBet = mostRecentBet;//the players mostRecentBet is set equal to the global mostRecentBet so that the players mostRecentBet is updated
        currentPlayer.numOfChips -= mostRecentBet;//the players total number of chips has the raise value subtracted from it so that the players cumulative number of chips is updated
        totalChipsInPot +=currentPlayer.mostRecentBet;//the players total numberOfChipsInPot has the playersMostRecentBet added to it so that the player cumulative bet in the pot is updated
        currentPlayer.hasRaised = true;
        IncrementActivePlayer();// Go to next player
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
        CheckBettingStatus();
    }


    public void CallOnClick()
    {
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("call button working");
        currentPlayer.mostRecentBet = mostRecentBet;//the players mostRecentBet is set equal to the previous global mostRecentBet
        currentPlayer.numOfChips -= currentPlayer.mostRecentBet;//the players cumulative amount of chips has their mostRecentBet subtracted from it
        currentPlayer.numOfChipsInPot +=currentPlayer.mostRecentBet;//the players cumulative bet in the current pot has their mostRecentBet added to it
        totalChipsInPot += currentPlayer.mostRecentBet;
        currentPlayer.hasCalled = true;
        DebugPrint("most recent bet", currentPlayer.mostRecentBet);
        IncrementActivePlayer();///increases active player by one position
        DebugPrint("current active player is at postion", activePlayerPosition);
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
        CheckBettingStatus();

    }
    public void FoldOnClick()
    {
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        print("fold button working");
        currentPlayer.hasFolded = true;
        if (currentPlayer.hasFolded == true)
        {
            currentPlayer.gameObject.SetActive(false);
            DebugPrint("what is the activePlayerPosition", activePlayerPosition);
            IncrementActivePlayer();
            //bugs, must make sure the folded players can no longer be interacted with i.e. they cannot be rotated to anymore and instead the rotation will skip to the next non folded player
        }
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
        CheckBettingStatus();
    }
    public void DistributePot()//will be run when players cards are evaluated or everyone folds
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        currentPlayer.numOfChips += potValue;//sets potValue to 0, sets numOfChipsInPot and adds to numOfChips on playerClassScript of player who won
        gameNumber++;
    }

    public void CheckBettingStatus() //this function must be responsible for checking whether or not the round may increase, it must check whether or not the big-blind players bet is equal to the most recent bet (the little blinds bet), if it is not then the game will keep looping
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        playerClassScript bigBlindPlayer = players[(1 + gameNumber) % numberPlayers].GetComponent<playerClassScript>();
        if (bigBlindPlayer.hasCalled==true || bigBlindPlayer.hasFolded == true)
        {
            round+=1;

            potValue += totalChipsInPot;
            potValueText.text = Convert.ToString(potValue);
            DebugPrint("round value is ", round); //checking what the round is, as the flop was not being dealt.
            bigBlindPlayer.hasCalled = false;//resets the values for bigBlind and littleBlind back to false
            bigBlindPlayer.hasFolded = false;
            GameLoop();
        }
        else if(bigBlindPlayer.hasRaised == true)
        {
            //keep loopijg, null?
        }
    }
    public void IncrementActivePlayer()
    {
        DebugPrint("active playe running", activePlayerPosition);
        bool flag = true;//keeps track of loop running
        while(flag)
        {
            activePlayerPosition = (activePlayerPosition + 1) % (numberPlayers);
            DebugPrint(",", activePlayerPosition);
            playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
            DebugPrint("hasfoeld?", currentPlayer.hasFolded);
            if(currentPlayer.hasFolded == false)
            {
                flag = false;//loop stops
            }
        }
        DebugPrint("from inside IncrementPlayer function", activePlayerPosition);
    }
    public void evaluateHand()
    {
        
        for (int i = 0; i < players.Count; i++)
        {

            List<GameObject> joinedList = flopList.Concat(players[i].GetComponent<playerClassScript>().cards).ToList();//joins both flopList cards and the list of cards on the player
            //loops through each player and determines if they have any of the hand combinations in joinedList
            checkHand(joinedList);
            //takes highest card value 
        }
    }
    public void playerHasPair(List<GameObject> joinedList)//returns boolean value so that in the 
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        //parse list into the is function, returns to player
        //check list for two same cardsz
        bool hasPair = false;
        //if true then hasPair = true;

    }
    public void playerHasHighCard(List<GameObject> joinedList)
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        bool hasHighCard = false;
        //check list for ace, king, queen, jack
        if(joinedList.Contains())//must check whether the list contains an ace || king|| queen|| jack, possibly by matching the icon face string ("1") e.g. for ace
        {
            hasHighCard = true;
            currentPlayer.valueOfCardsInHand = 1;
        }
        //if true then hasHighCard = true;
    }
    public void playerHasTwoPair(List<GameObject> joinedList)
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        bool hasTwoPair = false;
        //check list for two different pairs
        //if true then hasTwoPair = true;
    }
    public void playerHasThreeOfAKind(List<GameObject> joinedList)
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        bool hasThreeOfAKind = false;
        //check list for 3 cards that are the same
        //if true then hasThreeOfAKind = true
    }
    public void playerHasFourOfAKind(List<GameObject> joinedList)
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        bool hasFourOfAKind = false;
        //check list for 4 cards of the same value
        //if true then hasFourOfAKind = true;
    }
    public void playerHasStraight(List<GameObject> joinedList)
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        bool hasStraight = false;
        //check list for 5 cards in increasing order
        //if true then hasStraight = true;
    }
    public void playerHasFullHouse(List<GameObject> joinedList)//only will run if three of a kind and a pair returns true
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        bool hasFullHouse = false;
        //check list for 3 cards of the same value, check list for a pair
        //if true then hasFullHouse = true;
    }
    public void playerHasFlush(List<GameObject> joinedList)
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        bool hasFlush = false;
        //check if 5 cards are from the same suit
        //if true then hasFlush = true;
    }
    public void playerHasStraightFlush(List<GameObject> joinedList)
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        bool hasStraightFlush = false;
        //check is list has 5 cards of increasing value of the same suit
        //if true then hasStraightFlush = true
    }
    public void playerHasRoyalFlush(List<GameObject> joinedList)
    {
        playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>();
        bool hasRoyalFlush = false;
        //chekc if the list has ace, king, queen, jack, 10
        //if true then royalFlush = true;
    }
    public void checkHand(List<GameObject> joinedList)
    {
        //runs all the functions checking in order

    }
    //each function will check if the player has one of these hands, it will return true if it does and increment to the next check, if not it will increment to the next check
    //if one player has more than one hand, it will take the highest possible integer value found as the players hand
}