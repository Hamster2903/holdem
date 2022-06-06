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
    public int gameNumber;
    public int totalChipsInPot;
    public int activePlayerPosition = 0;
    public bool allFolded = false;
    public bool potOver = false;
    public List<GameObject> players;
    public GameObject playerPrefab;
    //public GameObject cardGroups;
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
        GeneratePlayers(5, 1);
        deckScript.Generate();
        deckScript.Shuffle();
        GeneratePlayerObjects();
        DealToHands();
        
    }
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
            round = 0;
            StartNextGame();
            
            //reset the game, clear all cards from players, clear cards from deck, reshuffle, re-deal cards to players
        }
    }
    public void GeneratePlayers(int numPlayers, int gameNum)
    {
        gameNumber = gameNum;
        players.Clear();
        for (int i = 0; i < numPlayers; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            players.Add(newPlayer);
            newPlayer.gameObject.GetComponent<Image>().enabled = false;

        }
        for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
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
        DebugPrint("ENABLING IMAGE IN GENERATE PLAYERS", 0);
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
        
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

            }
         
        }
        
    }
    
    public void GeneratePlayerObjects()
    {
        int count = 0;
        
        GameObject playerPositionsParent = playerPositions[players.Count - 3];//defines player position in objects as its position in the list
        foreach (Transform item in playerPositionsParent.transform)//for each position in the list instantiates players
        {
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
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        string raiseInputText = raiseInput.text;//sets the string value recorded in the input text to an integer which will be used to represent features on the player script
        raiseValue = int.Parse(raiseInputText) +mostRecentBet*2;//raise value is equal to the players input + 2 times the most recent bet because to raise the bet it must be atleast two times the previous bet
        mostRecentBet = raiseValue;
        currentPlayer.mostRecentBet = mostRecentBet;//the players mostRecentBet is set equal to the global mostRecentBet so that the players mostRecentBet is updated
        currentPlayer.numOfChips -= mostRecentBet;//the players total number of chips has the raise value subtracted from it so that the players cumulative number of chips is updated
        totalChipsInPot +=currentPlayer.mostRecentBet;//the players total numberOfChipsInPot has the playersMostRecentBet added to it so that the player cumulative bet in the pot is updated
        currentPlayer.hasRaised = true;
        currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
        IncrementActivePlayer();// Go to next player
        CheckIfPlayerHasValidChips();
        CheckAllFolded();
        CheckBettingStatus();
    }


    public void CallOnClick()
    {

        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        currentPlayer.mostRecentBet = mostRecentBet;//the players mostRecentBet is set equal to the previous global mostRecentBet
        currentPlayer.numOfChips -= currentPlayer.mostRecentBet;//the players cumulative amount of chips has their mostRecentBet subtracted from it
        currentPlayer.numOfChipsInPot +=currentPlayer.mostRecentBet;//the players cumulative bet in the current pot has their mostRecentBet added to it
        totalChipsInPot += currentPlayer.mostRecentBet;
        currentPlayer.hasCalled = true;
        currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
        IncrementActivePlayer();///increases active player by one position
        
        CheckIfPlayerHasValidChips();
        CheckAllFolded();
        CheckBettingStatus();

    }
    public void FoldOnClick()
    {
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();

        currentPlayer.hasFolded = true;
        if (currentPlayer.hasFolded == true)
        {
            currentPlayer.gameObject.SetActive(false);
            IncrementActivePlayer();
        }
        
        CheckIfPlayerHasValidChips();
        CheckAllFolded();
        CheckBettingStatus();
    }
    public void DistributePot()//will be run when players cards are evaluated or everyone folds
    {
        playerClassScript winningPlayer = players[players.Count - 1].GetComponent<playerClassScript>();
        winningPlayer.numOfChips += potValue;//sets potValue to 0, sets numOfChipsInPot and adds to numOfChips on playerClassScript of player who won
        winningPlayer.playerChipsText.text = Convert.ToString(winningPlayer.numOfChips);
        gameNumber++;
        round = 0;
        
    }
    public void StartNextGame()
    {
        //make this reset the game to original state but keep each players chips constant
        //reset all boolean to orginal value
        allFolded = false;
        for (int i = 0; i < players.Count; i++)//for loop to loop through each player and set everything back to normal
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            currentPlayer.gameObject.SetActive(true);
            currentPlayer.numOfChipsInPot = 0;
            currentPlayer.hasFolded = false;
            currentPlayer.hasRaised = false;
            currentPlayer.hasCalled = false;
            currentPlayer.mostRecentBet = 0;
        }

        //round =0
        round = 0;
        //potValue = 0
        potValue = 0;
        //totalChipsInPot = 0
        totalChipsInPot = 0;
        //most recent bet = 0
        mostRecentBet = 0;
        //clear cards from players hand
        foreach (GameObject player in players)
        {
            playerClassScript newPlayer = player.GetComponent<playerClassScript>();
            foreach (GameObject card in newPlayer.cards)
            {
                GameObject.Destroy(card);
            }
        }
        playerPositions.Clear();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<playerClassScript>().cards.Clear();
        }
        //clear cards from flop list
        foreach (Transform item in flopGrid.transform)
        {
            Destroy(item.gameObject);
        }
        flopList.Clear();
        //clear player positions
        //regenerate deck, player hands and player positions

        deckScript.Generate();
        deckScript.Shuffle();
        ReGeneratePlayers(5,1);
        DealToHands();
        GameLoop();
    }
    public void ReGeneratePlayers(int numPlayers, int gameNum)
    {
        
        gameNumber = gameNum;
        /*for (int i = 0; i < numPlayers; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            players.Add(newPlayer);
            newPlayer.gameObject.GetComponent<Image>().enabled = false;

        }*/
        for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
        }
        playerClassScript dealerPlayer = players[(-1 + gameNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript littleBlindPlayer = players[(0 + gameNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript bigBlindPlayer = players[(1 + gameNum) % numPlayers].GetComponent<playerClassScript>();
        dealerPlayer.isDealer = true;//set position 0 to dealer button, position 1 to little blind, position 2 to big blind
        littleBlindPlayer.isLittleBlind = true;
        bigBlindPlayer.isBigBlind = true;
        littleBlindPlayer.mostRecentBet = 5;
        potValue += littleBlindPlayer.mostRecentBet;
        bigBlindPlayer.mostRecentBet = 10;
        potValue += bigBlindPlayer.mostRecentBet;
        mostRecentBet = bigBlindPlayer.mostRecentBet;
        potValueText.text = Convert.ToString(potValue);
        activePlayerPosition = (2 + gameNum) % numPlayers;
        DebugPrint("CHANGING PCITRUE IN REGERNATION", 0);
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
    }
    public void CheckBettingStatus() //this function must be responsible for checking whether or not the round may increase, it must check whether or not the big-blind players bet is equal to the most recent bet (the little blinds bet), if it is not then the game will keep looping
    {
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        playerClassScript bigBlindPlayer = players[(1 + gameNumber) % players.Count].GetComponent<playerClassScript>();
        if (bigBlindPlayer.hasCalled==true || bigBlindPlayer.hasFolded == true)
        {
            round+=1;
            potValue += totalChipsInPot;
            potValueText.text = Convert.ToString(potValue);
            bigBlindPlayer.hasCalled = false;//resets the values for bigBlind and littleBlind back to false
            bigBlindPlayer.hasFolded = false;
            GameLoop();
        }
        else if(bigBlindPlayer.hasRaised == true)
        {
            //keep loopijg, null?
        }
    }
    public void CheckAllFolded()
    {
        int count = 0;
        for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();

            if (currentPlayer.hasFolded == true)
            {
                count += 1;
            }
            
        }
        if(count == players.Count-1)
        {
            allFolded = true;
        }
        else
        {
            allFolded = false;
        }
        if(allFolded == true)
        {
            //set player that is remaining to winning player
            DebugPrint("all has folded", 0);
            DistributePot();
            StartNextGame();
        }
    }
    public void CheckIfPlayerHasValidChips()
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            if(currentPlayer.numOfChips <=0)
            {
                players.RemoveAt(i);
                Destroy(currentPlayer);
            }
        }
    }
    public void CheckIfGameShouldEnd()
    {
        //chekc if there is 1 player in the list
        //
        if(players.Count == 1)
        {

        }
    }
    public void IncrementActivePlayer()
    {
        if (round == 4 || allFolded)
        {
            return; // TODO: figure this shit out
        }
        bool flag = true; //keeps track of loop running
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        DebugPrint("disabling active player", activePlayerPosition);
        while (flag)
        {
            activePlayerPosition = (activePlayerPosition + 1) % (players.Count);
            DebugPrint("playwr count is",players.Count);
            DebugPrint("incremented active player to", activePlayerPosition);
            playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
            if(currentPlayer.hasFolded == false)
            {
                DebugPrint("enabling active player", activePlayerPosition);
                players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
                flag = false;//loop stops
            }
        }
    }
    public void EvaluateHand()//runs GetHandRank for each player
    {
        
        for (int i = 0; i < players.Count; i++)
        {
            List<GameObject> handList = flopList.Concat(players[i].GetComponent<playerClassScript>().cards).ToList();//joins both flopList cards and the list of cards on the player
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>(); 
            currentPlayer.valueOfCardsInHand = GetHandRank(handList);
        }
        SortPlayersByHandRank();
        /*for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            DebugPrint("item in list", currentPlayer.valueOfCardsInHand);
        }*/
        DistributePot();

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
    public List<GameObject> SortHandByFacePower(List<GameObject> handList)
    {
        handList.Sort(CompareFaceByPower);
        return handList;
    }
    public void SortPlayersByHandRank()
    {
        players.Sort(CompareHandByRank);
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
                if ((GetFacePower(handList[n]) == currentFace + 1
                || (nestedCardScript.face == "Ace"
                    && currentFace + 1 == 14)) && nestedCardScript.suit == currentSuit)
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
                if(GetFacePower(handList[n]) == currentFace+1
                || (nestedCardScript.face == "Ace" 
                    && currentFace + 1 == 14))
                // (false && true) || true
                // (3 / 4) + 5
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
        
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = GetFacePower(handList[i]);
            
            if(face == excludedFace)
            {
                
                continue;
            }
            currentCount = 1;
            for (int n = i+1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                
                if (GetFacePower(handList[n]) == currentFace)
                {
                    currentCount += 1;
                    if(currentCount==2)
                    {
                        if(excludedFace == "")
                        {
                           
                            return isTwoPair(handList, nestedCardScript.face);
                        }
                        else
                        {
                            
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
        handList = SortHandByFacePower(handList);
        if (isRoyalFlush(handList))
        {
            return 10;
        }
        else if (isStraightFlush(handList))
        {
            return 9;
        }
        else if (isFourOfAKind(handList))
        {
            return 8;
        }
        else if (isFullHouse(handList))
        {
            return 7;
        }
        else if (isFlush(handList))
        {
            return 6;
        }
        else if (isStraight(handList))
        {
            return 5;
        }
        else if (isThreeOfAKind(handList))
        {
            return 4;
        }
        else if (isTwoPair(handList, ""))
        {
            return 3;
        }
        else if (isPair(handList))
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
}