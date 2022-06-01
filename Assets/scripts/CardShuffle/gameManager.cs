using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
       
    public DeckScript deckScript;
    public CardScript cardScript;
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
        if(round == 4)
        {
            EvaluateHand();
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
    public void EvaluateHand()//runs GetHandRank for each player
    {
        
        for (int i = 0; i < players.Count; i++)
        {
            List<GameObject> handList = flopList.Concat(players[i].GetComponent<playerClassScript>().cards).ToList();//joins both flopList cards and the list of cards on the player
            playerClassScript currentPlayer = players[activePlayerPosition % players.Count].GetComponent<playerClassScript>(); 
            currentPlayer.valueOfCardsInHand =GetHandRank(handList);
            DebugPrint("current value of cards in hand", currentPlayer.valueOfCardsInHand);
            
        }
        
    }

    public int GetFacePower(GameObject currentCard)
    {
        CardScript currentCardScript = currentCard.GetComponent<CardScript>();
        var facePowerDictionary = new Dictionary<string, int>();
        facePowerDictionary.Add("Ace", 1);
        facePowerDictionary.Add("2", 2);
        facePowerDictionary.Add("3", 3);
        facePowerDictionary.Add("4", 4);
        facePowerDictionary.Add("5", 5);
        facePowerDictionary.Add("6", 6);
        facePowerDictionary.Add("7", 7);
        facePowerDictionary.Add("8", 8);
        facePowerDictionary.Add("9", 9);
        facePowerDictionary.Add("10", 10);
        facePowerDictionary.Add("Jack", 11);
        facePowerDictionary.Add("Queen", 12);
        facePowerDictionary.Add("King", 13);
        return facePowerDictionary[currentCardScript.face];
    }
    public int CompareHandByRank(GameObject player1, GameObject player2)
    {
        playerClassScript currentPlayer1 = player1.GetComponent<playerClassScript>();
        playerClassScript currentPlayer2 = player2.GetComponent<playerClassScript>();
        int player1Rank= currentPlayer1.valueOfCardsInHand;
        int player2Rank = currentPlayer2.valueOfCardsInHand;
        if (player1Rank > player2Rank)
        {
            return 1;
        }
        else if (player2Rank > player1Rank)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
    private int CompareFaceByPower(GameObject card1, GameObject card2)
    {
        int face1Power = GetFacePower(card1);
        int face2Power = GetFacePower(card2);

        if (face1Power > face2Power)
        {
            return 1;
        }
        else if (face2Power > face1Power)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public List<GameObject> sortHandByFacePower(List<GameObject> handList)
    {
        handList.Sort(CompareFaceByPower);
        return handList;
    }
    public List<GameObject> SortPlayerByHandRank(List<GameObject> handList)
    {

    }
    public int GetNumberOfSuitInHand(List<GameObject> handList, string targetSuit)
    {
        
        int count = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            if(suit == targetSuit)
            {
                count += 1;
            }
        }
        return count;
    }
    public int GetNumberOfFaceInHand(List<GameObject> handList, string targetFace)
    {
        int count = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string face = currentCardScript.face;
            if (face == targetFace)
            {
                count += 1;
            }
        }
        return count;
    }
    public bool isRoyalFlush(List<GameObject> handList)
    {
        if(GetNumberOfFaceInHand(handList, "Ace") == 1 && GetNumberOfFaceInHand(handList, "King") ==1 && GetNumberOfFaceInHand(handList, "Queen") ==1 && GetNumberOfFaceInHand(handList, "Jack") == 1 && GetNumberOfFaceInHand(handList, "10") ==1 && GetNumberOfSuitInHand(handList, "Clubs") >=5 || GetNumberOfSuitInHand(handList, "Diamonds") >= 5 || GetNumberOfSuitInHand(handList, "Spades") >= 5 || GetNumberOfSuitInHand(handList, "Hearts") >= 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool isStraightFlush(List<GameObject> handList)
    {
        
        for (int i = 0; i < handList.Count; i++)
        {
            int currentCount = 0;
            int currentFace = 0;
            string currentSuit = "";
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = GetFacePower(handList[i]);
            currentSuit = suit;
            currentCount = 1;
            for (int n = i+1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if(GetFacePower(handList[n])==currentFace+1
                    &&  nestedCardScript.suit == currentSuit)
                {
                    currentCount +=1;
                    currentFace = GetFacePower(handList[n]);
                    if(currentCount ==5)
                    {
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

        }
        return false;
    }
    public bool isFourOfAKind(List<GameObject> handList)
    {
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = GetFacePower(handList[i]);
            currentCount = 1;
            for (int n = i+1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if(GetFacePower(handList[n]) == currentFace)
                {
                    currentCount += 1;
                    if(currentCount==4)
                    {
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }

            }
        }
        return false;
    }
    public bool isFlush(List<GameObject> handList)
    {
        if(GetNumberOfSuitInHand(handList, "Diamonds") >= 5
  || GetNumberOfSuitInHand(handList, "Spades") >= 5
  || GetNumberOfSuitInHand(handList, "Clubs") >= 5
  || GetNumberOfSuitInHand(handList, "Hearts") >= 5
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool isStraight(List<GameObject> handList)
    {
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = GetFacePower(handList[i]);
            currentCount = 1;
            for (int n = i+1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if(GetFacePower(handList[n]) ==currentFace+1)
                {
                    currentCount += 1;
                    currentFace = GetFacePower(handList[n]);
                    if(currentCount ==5)
                    {
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        return false;
    }
    public bool isFullHouse(List<GameObject> handList)
    {
        string[] possibleFaces = new string[] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };
        string alreadyUsedFace = "";
        for (int i = 0; i < possibleFaces.Length; i++)
        {
            bool foundMultipleFaces = false;
            for (int n = 0; n < 13; n++)
            {
                if(alreadyUsedFace == "")
                {
                    if(GetNumberOfFaceInHand(handList, possibleFaces[n]) >=3)
                    {
                        alreadyUsedFace = possibleFaces[n];
                        foundMultipleFaces = true;
                        break;
                    }

                }
                else
                {
                    if(possibleFaces[n] != alreadyUsedFace&&GetNumberOfFaceInHand(handList, possibleFaces[n])>=2)
                    {
                        return true;
                    }
                }
            }
            if(!foundMultipleFaces)
            {
                alreadyUsedFace = "";
            }
        }
        return false;
    }
    public bool isThreeOfAKind(List<GameObject> handList)
    {
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = GetFacePower(handList[i]);
            currentCount = 1;
            for (int n = i+1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if(GetFacePower(handList[n]) == currentFace)
                {
                    currentCount += 1;
                    if(currentCount ==3)
                    {
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        return false;
    }
    public bool isTwoPair(List<GameObject> handList, string excludedFace)
    {
        DebugPrint("Running function", excludedFace);
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = GetFacePower(handList[i]);
            DebugPrint("Currentfacepower", currentFace);
            if(face == excludedFace)
            {
                print("Currentface is excluded");
                continue;
            }
            currentCount = 1;
            for (int n = i+1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                DebugPrint("nestedFacePower", GetFacePower(handList[n]));
                if (GetFacePower(handList[n]) == currentFace)
                {
                    currentCount += 1;
                    if(currentCount==2)
                    {
                        if(excludedFace == "")
                        {
                            print("Recursing");
                            return isTwoPair(handList, nestedCardScript.face);
                        }
                        else
                        {
                            print("returning trrue");
                            return true;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        return false;
    }
    public bool isPair(List<GameObject> handList)
    {
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = GetFacePower(handList[i]);
            currentCount = 1;
            for (int n = 0; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                
                if(GetFacePower(handList[n]) == currentFace)
                {
                    currentCount += 1;
                    if(currentCount ==2)
                    {
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        return false;
    }
    public int GetHandRank(List<GameObject> handList)
    {
        handList = sortHandByFacePower(handList);
        if (isRoyalFlush(handList))
        {
            return 1;
        }
        else if (isStraightFlush(handList))
        {
            return 2;
        }
        else if (isFourOfAKind(handList))
        {
            return 3;
        }
        else if (isFullHouse(handList))
        {
            return 4;
        }
        else if (isFlush(handList))
        {
            return 5;
        }
        else if (isStraight(handList))
        {
            return 6;
        }
        else if (isThreeOfAKind(handList))
        {
            return 7;
        }
        else if (isTwoPair(handList, ""))
        {
            return 8;
        }
        else if (isPair(handList))
        {
            return 9;
        }
        else
        {
            return 10;
        }
    }
}