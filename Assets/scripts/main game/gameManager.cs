using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public int handNumber;
    public int numberOfOriginalPlayers;
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
    //allows easier to understand debugging
    public void DebugPrint(string prefix, object message)
    {
        if(debug)
        {
            print(prefix + ": " + message);
        }
    }
    void Start()
    {
        GeneratePlayers(PlayerPrefs.GetInt("players"), 1);
        deckScript.Generate();
        deckScript.Shuffle();
        GeneratePlayerObjectsAroundTable();
        DealToHands();
    }
    //is used to keep track of the rounds and what occurs in each
    public void RoundLoop()
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
            StartNextHand();

        }
    }
    //generates an amount of players from 3 to 5 and gives them specific positional roles, i.e. it sets a player as a littleBlind player and gives their class variables values
    public void GeneratePlayers(int numPlayers, int handNum)
    {
        handNumber = handNum;
        numberOfOriginalPlayers = numPlayers;
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
        playerClassScript dealerPlayer = players[(-1 + handNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript littleBlindPlayer = players[(0 + handNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript bigBlindPlayer = players[(1 + handNum) % numPlayers].GetComponent<playerClassScript>();
        dealerPlayer.isDealer = true;//set position 0 to dealer button, position 1 to little blind, position 2 to big blind
        littleBlindPlayer.isLittleBlind = true;
        bigBlindPlayer.isBigBlind = true;
        littleBlindPlayer.mostRecentBet =5;
        potValue += littleBlindPlayer.mostRecentBet;
        bigBlindPlayer.mostRecentBet = 10;
        potValue += bigBlindPlayer.mostRecentBet;
        mostRecentBet = bigBlindPlayer.mostRecentBet;
        potValueText.text = Convert.ToString(potValue);
        activePlayerPosition = (2 + handNum) % numPlayers;
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
        
    }
    //deals two card game objects from the deck to the players hands, also adds the cards and their values to the players specific card list
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
    //generates the playerPrefab objects in the specified arrangement around the table image at either position 123, 1234,12345
    public void GeneratePlayerObjectsAroundTable()
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
    //removes cards from the deck list and then adds them to the flop list
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
    //removes cards from the deck list and then adds them to the flop list
    public void DealToTurn()
    {
        GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
        deckScript.cards.Remove(cardToMove); //removes card from list
        flopList.Add(cardToMove);
        cardToMove.transform.SetParent(flopGrid.transform);
    }
    //removes cards from the deck list and then adds them to the flop list
    public void DealToRiver()
    {
        GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
        deckScript.cards.Remove(cardToMove); //removes card from list
        flopList.Add(cardToMove);
        cardToMove.transform.SetParent(flopGrid.transform);
    }
    //allows the player to raise their bet to a specified integer amount from their amount of chips and then incrementing the player by one
    public void RaiseOnClick()
    {
        print("RaiseOnClick");
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        string raiseInputText = raiseInput.text;//sets the string value recorded in the input text to an integer which will be used to represent features on the player script
        bool isAValidRaiseInput;
        int a;
        isAValidRaiseInput = int.TryParse(raiseInputText, out a);
        if (!isAValidRaiseInput || int.Parse(raiseInputText) > currentPlayer.numOfChips)
        {
            raiseInput.text = "please input a valid number!";
        }
        else
        {
            if (int.Parse(raiseInput.text) <= mostRecentBet)
            {
                CallOnClick();
                return;
            }
            currentPlayer.hasRaised = true;
            players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
            raiseValue = int.Parse(raiseInputText) + mostRecentBet * 2;//raise value is equal to the players input + 2 times the most recent bet because to raise the bet it must be atleast two times the previous bet
            mostRecentBet = raiseValue;
            currentPlayer.mostRecentBet = mostRecentBet;//the pslayers mostRecentBet is set equal to the global mostRecentBet so that the players mostRecentBet is updated
            currentPlayer.numOfChips -= currentPlayer.mostRecentBet;//the players total number of chips has the raise value subtracted from it so that the players cumulative number of chips is updated
            CheckIfPlayerIsAlreadyAllIn();
            CheckIfPlayerIsAllIn();
            //this chunk of code runs the raise calculations if the currentPlayer is not all in and has not gone all in previously
            if (currentPlayer.hasGoneAllIn == false || currentPlayer.isAllIn == false)
            {
                RaiseCalculations();

            }
            AddChipsToPot();
            CheckAllFolded();
            if(!CheckIfRoundCanIncrement())
            {
                IncrementActivePlayer();
            }

        }
    }
    //this updates the pot value text
    public void AddChipsToPot()
    {
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        potValue += mostRecentBet;
        potValueText.text = Convert.ToString(potValue);
    }
    //placed raise calculations in function so broader function is easier to understand
    public void RaiseCalculations()
    {
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        totalChipsInPot += currentPlayer.mostRecentBet;//the players total numberOfChipsInPot has the playersMostRecentBet added to it so that the player cumulative bet in the pot is updated
        
        currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
    }
    //allows the player to match the previous bet from their amount of chips and then incrementing the player by one
    public void CallOnClick()
    {
        print("CallOnClick");
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        currentPlayer.hasCalled = true;
        currentPlayer.mostRecentBet = mostRecentBet;//the players mostRecentBet is set equal to the previous global mostRecentBet
        currentPlayer.numOfChips -= currentPlayer.mostRecentBet;//the players cumulative amount of chips has their mostRecentBet subtracted from it
        CheckIfPlayerIsAlreadyAllIn();//checks if the player is already all in from a prvious bet, if they are increments to next player skipping their turn
        CheckIfPlayerIsAllIn();//chceks if player is going to go all in with their current bet they are trying, if they are then it updates values and skips to next player
        //this block of code runs if the other two functions are not successful and will complete the required action
        if((currentPlayer.hasGoneAllIn== false) || (currentPlayer.isAllIn = false))
        {
            CallCalculations();
        }
        AddChipsToPot();
        CheckAllFolded();
        if (!CheckIfRoundCanIncrement())
        {
            IncrementActivePlayer();//increases active player by one position
        }
    
    }
    public void CallCalculations()
    {
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        currentPlayer.numOfChipsInPot += currentPlayer.mostRecentBet;
        totalChipsInPot += currentPlayer.mostRecentBet;
        
        currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
    }
    //folds the players hand and removes them from the bettiing for remainder of the hand
    public void FoldOnClick()
    {
        print("FoldOnClick");
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        currentPlayer.hasFolded = true;
        print(currentPlayer.hasFolded);
        DebugPrint("player"+activePlayerPosition+ "has folded", currentPlayer.hasFolded);
        currentPlayer.gameObject.SetActive(false);
        IncrementActivePlayer();
        CheckIfRoundCanIncrement();
        CheckAllFolded();
    }
    //will add the amount of chips in the pot to the winning player determined by the evaluatehand function
    public void DistributePotAtHandEvaluation()//will be run when players cards are evaluated or everyone folds
    {
        print("DistributePotAtHandEvaluation");
        playerClassScript winningPlayer = players[players.Count - 1].GetComponent<playerClassScript>();
        winningPlayer.numOfChips += potValue;//sets potValue to 0, sets numOfChipsInPot and adds to numOfChips on playerClassScript of player who won
        winningPlayer.playerChipsText.text = Convert.ToString(winningPlayer.numOfChips);
        handNumber++;
        round = 0;
    }
    //resets all the boolean values that are unimportant for the players identification, i.e. everything except player name and numOfChips
    public void StartNextHand()
    {
        print("startNextHand");
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
        round = 0;
        potValue = 0;
        totalChipsInPot = 0;
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
        deckScript.Generate();
        deckScript.Shuffle();
        ReGeneratePlayers(numberOfOriginalPlayers,handNumber);
        DealToHands();
        RoundLoop();
    }
    //this function regenerates the players with the new values required, i.e. a new little blind and big blind player is created, new original active player is defined
    public void ReGeneratePlayers(int numPlayers, int handNum)
    {
        print("Regenerateplayers");
        for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
            players[i].gameObject.GetComponent<Image>().enabled = false;
        }
        playerClassScript dealerPlayer = players[(-1 + handNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript littleBlindPlayer = players[(0 + handNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript bigBlindPlayer = players[(1 + handNum) % numPlayers].GetComponent<playerClassScript>();
        dealerPlayer.isDealer = true;//set position 0 to dealer button, position 1 to little blind, position 2 to big blind
        littleBlindPlayer.isLittleBlind = true;
        bigBlindPlayer.isBigBlind = true;
        littleBlindPlayer.mostRecentBet = 5;
        potValue += littleBlindPlayer.mostRecentBet;
        bigBlindPlayer.mostRecentBet = 10;
        potValue += bigBlindPlayer.mostRecentBet;
        mostRecentBet = bigBlindPlayer.mostRecentBet;
        potValueText.text = Convert.ToString(potValue);
        activePlayerPosition = (handNum + 2) % numPlayers;
        print(activePlayerPosition);
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
    }
    public bool CheckIfRoundCanIncrement() //this function must be responsible for checking whether or not the round may increase, it must check whether or not the big-blind players bet is equal to the most recent bet (the little blinds bet), if it is not then the game will keep looping
    {
        print("checkIfroundCanIncrement");
        playerClassScript bigBlindPlayer = players[(1 + handNumber) % players.Count].GetComponent<playerClassScript>();
        if (bigBlindPlayer.hasCalled==true || bigBlindPlayer.hasFolded == true)
        {
            round+=1;
            RoundLoop();
            bigBlindPlayer.hasCalled = false;//resets the values for bigBlind and littleBlind back to false
            return true;
        }
        else if(bigBlindPlayer.hasRaised == true)
        {
            return false;
        }
        return false;

    }
    //checks if every player except 1 is folded
    public void CheckAllFolded()
    {
        print("CheckAllFolded");
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
            for (int i = 0; i < players.Count; i++)
            {
                playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
                if(currentPlayer.hasFolded == false)
                {
                    DistributePotIfFold(i);
                    CheckIfPlayerIsValid(i);
                }
            }
            CheckIfGameShouldEnd();
            StartNextHand();
        }
    }
    public void DistributePotIfFold(int winningPlayerInt)
    {
        print("DistributePotAtHandEvaluation");
        playerClassScript winningPlayer = players[winningPlayerInt].GetComponent<playerClassScript>();
        winningPlayer.numOfChips += potValue;//sets potValue to 0, sets numOfChipsInPot and adds to numOfChips on playerClassScript of player who won
        winningPlayer.playerChipsText.text = Convert.ToString(winningPlayer.numOfChips);
        handNumber++;
        round = 0;
    }
    public void CheckIfPlayerIsAllIn()
    {
        print("CheckIfPlayerGoesAllIn");
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        if (currentPlayer.mostRecentBet>=currentPlayer.numOfChips)
        {
            //resets the bet to the total amount of chips in the player
            raiseValue = currentPlayer.numOfChips;
            currentPlayer.mostRecentBet = currentPlayer.numOfChips+=mostRecentBet;
            mostRecentBet=currentPlayer.numOfChips;
            currentPlayer.numOfChips = 0;
            currentPlayer.isAllIn = true;
            currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
        }
    }
    public void CheckIfPlayerIsAlreadyAllIn()
    {
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        if(currentPlayer.isAllIn == true)
        {
            currentPlayer.hasGoneAllIn = true;
        }
    }
    //checks if the player is allowed to keep playing, not allowed if they fail an all in bet
    public void CheckIfPlayerIsValid(int winningPlayerInt)
    {
        print("CheckIfPlayerIsAValid");
        CheckAllFolded();
        for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            if (currentPlayer.numOfChips == 0 && i != winningPlayerInt) 
            {
                players.RemoveAt(i);
                currentPlayer.gameObject.SetActive(false);
                //movesd player indexd down 1
                i--;
                //moves the winning player down 1 to compensate for this as players are removed at i and the list lengthddecreases by one
                if (i < winningPlayerInt) 
                {
                    winningPlayerInt--;
                }
            }
        }
    }
    //this checks if there is 1 player in the list of players and sends the game to the game over scene
    public void CheckIfGameShouldEnd()
    {
        print("CheckIfGameShouldEnd");
        //chekc if there is 1 player in the list
        if(players.Count == 1)
        {
            SceneManager.LoadScene(4);
        }
    }
    //this increments the active player to 1 position along the list
    public void IncrementActivePlayer()
    {
        print("IncrementActivePlayer");
        
        bool flag = true; //keeps track of loop running
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        while (flag)
        {
            activePlayerPosition = (activePlayerPosition + 1) % (players.Count);
            playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
            if(currentPlayer.hasFolded == false)
            {
                players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
                flag = false;//loop stops
            }
        }
    }
    //gets and sets each players hand value to a number, sorts the players hand and distributes the pot
    public void EvaluateHand()
    {
        print("EvaluateHand");
        for (int i = 0; i < players.Count; i++)
        {
            List<GameObject> handList = flopList.Concat(players[i].GetComponent<playerClassScript>().cards).ToList();//joins both flopList cards and the list of cards on the player
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>(); 
            currentPlayer.valueOfCardsInHand = GetHandRank(handList);
        }
        CheckIfGameShouldEnd();
        SortPlayersByHandRank();
        DistributePotAtHandEvaluation();
        CheckIfPlayerIsValid(players.Count - 1);
    }
    //sets each card face string as equivalent to a number, uses the card game object as a parameter
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
    //compares hands by ranking of hand
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
    //compares cards in order of power
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
    //sorts the players hand in order of increasing face power
    public List<GameObject> SortHandByFacePower(List<GameObject> handList)
    {
        handList.Sort(CompareFaceByPower);
        return handList;
    }
    //sorts players in order of hand rank
    public void SortPlayersByHandRank()
    {
        players.Sort(CompareHandByRank);
    }
    //counts and returns the amount of a specific suit in a players hand
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
    //counts and returns the amount of a specific face in a players hand
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
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
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
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
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
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
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
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
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
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
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
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
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
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
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
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
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
    //this first sorts each players hand by its face rank and then returns an integer value
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