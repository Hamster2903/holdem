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
    public Text mostRecentBetText;
    public Text errorMessageText;
    public int potValue = 0;
    public int callValue;
    public int raiseValue;
    public int round = 0;
    public int mostRecentBet;
    public int handNumber;
    public int numberOfOriginalPlayers;
    public int activePlayerPosition = 0;
    public bool allFolded = false;
    public GameObject informationScene;
    public List<GameObject> players;
    public GameObject playerPrefab;
    //public GameObject cardGroups;
    public List<GameObject> flopList;
    public GameObject flopGrid;
    public List<GameObject> playerPositions;
    public bool debug = false;
    //activates the information scene over the top of the game scene
    public void swap_to_information_scene_on_click()
    {
        informationScene.SetActive(true);
    }
    //deactivates the information scene over the top of the game scene
    public void swap_back_to_game_scene_on_click()
    {
        informationScene.SetActive(false);
    }
    //allows easier to understand debugging, cant print a string and another message/object
    public void debug_print(string prefix, object message)
    {
        if (debug)
        {
            print(prefix + ": " + message);
        }
    }
    void Start()
    {
        generate_players(PlayerPrefs.GetInt("players"), 1);
        deckScript.Generate();
        deckScript.Shuffle();
        generate_player_objects_around_table();
        deal_to_hands();
    }
    //is used to keep track of the rounds and what occurs in each
    public void round_tracker()
    {
        print("RoundLoop");
        if (round == 1)
        {
            deal_to_flop();

        }
        if (round == 2)
        {
            deal_card_to_table();

        }
        if (round == 3)
        {
            deal_card_to_table();

        }
        if (round == 4)
        {
            evaluate_hand();
            round = 0;
            start_next_hand();

        }
    }
    //generates an amount of players from 3 to 5 and gives them specific positional roles, i.e. it sets a player as a littleBlind player and gives their class variables values
    public void generate_players(int numPlayers, int handNum)
    {
        print("GeneratePlayers");
        handNumber = handNum;
        numberOfOriginalPlayers = numPlayers;
        players.Clear();
        for (int i = 0; i < numPlayers; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            playerClassScript currentPlayer = newPlayer.GetComponent<playerClassScript>();
            currentPlayer.playerId = i;
            players.Add(newPlayer);
            newPlayer.gameObject.GetComponent<Image>().enabled = false;
        }
        //updates each players chip text
        for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
        }
        //set position 0 to dealer button, position 1 to little blind, position 2 to big blind
        playerClassScript dealerPlayer = players[(-1 + handNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript littleBlindPlayer = players[(0 + handNum) % numPlayers].GetComponent<playerClassScript>();
        playerClassScript bigBlindPlayer = players[(1 + handNum) % numPlayers].GetComponent<playerClassScript>();
        dealerPlayer.isDealer = true;
        littleBlindPlayer.isLittleBlind = true;
        bigBlindPlayer.isBigBlind = true;
        littleBlindPlayer.mostRecentBet = 5;
        littleBlindPlayer.numOfChips -= littleBlindPlayer.mostRecentBet;
        littleBlindPlayer.playerChipsText.text = Convert.ToString(littleBlindPlayer.numOfChips);
        potValue += littleBlindPlayer.mostRecentBet;
        bigBlindPlayer.mostRecentBet = 10;
        bigBlindPlayer.numOfChips -= bigBlindPlayer.mostRecentBet;
        bigBlindPlayer.playerChipsText.text = Convert.ToString(bigBlindPlayer.numOfChips);
        potValue += bigBlindPlayer.mostRecentBet;
        mostRecentBet = bigBlindPlayer.mostRecentBet;
        mostRecentBetText.text = Convert.ToString(mostRecentBet);
        potValueText.text = Convert.ToString(potValue);
        activePlayerPosition = (2 + handNum) % numPlayers;
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
    }
    //gets the players id for each player
    public playerClassScript get_player_by_id(int playerId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            if (currentPlayer.playerId == playerId)
            {
                return currentPlayer;
            }
        }
        return null;
    }
    //deals two card game objects from the deck to the players hands, also adds the cards and their values to the players specific card list
    public void deal_to_hands()
    {
        print("DealToHands");
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
    public void generate_player_objects_around_table()
    {
        print("GeneratePlayerObjectsAroundTable");
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
    public void deal_to_flop()
    {
        print("DealToFlop");
        for (int i = 0; i < 3; i++)
        {
            GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
            deckScript.cards.Remove(cardToMove); //removes card from list
            flopList.Add(cardToMove);//adds removed card to lsit
            cardToMove.transform.SetParent(flopGrid.transform);//puts card in game object grid
        }
    }
    //removes cards from the deck list and then adds them to the flop list
    public void deal_card_to_table()
    {
        print("DealCardToTable");
        GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
        deckScript.cards.Remove(cardToMove); //removes card from list
        flopList.Add(cardToMove);
        cardToMove.transform.SetParent(flopGrid.transform);
    }
    //allows the player to raise their bet to a specified integer amount from their amount of chips and then incrementing the player by one
    public void raise_on_click()
    {
        print("RaiseOnClick");
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        string raiseInputText = raiseInput.text;//sets the string value recorded in the input text to an integer which will be used to represent features on the player script
        bool isAValidRaiseInput;
        int a;
        isAValidRaiseInput = int.TryParse(raiseInputText, out a);
        if (!isAValidRaiseInput || int.Parse(raiseInputText) > currentPlayer.numOfChips)
        {
            errorMessageText.text = "please input a valid number!";
        }
        else
        {
            if (int.Parse(raiseInput.text) <= mostRecentBet)
            {
                call_on_click();
                return;
            }
            raiseInput.text = "";
            errorMessageText.text = "";
            currentPlayer.hasRaised = true;
            players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
            raiseValue = int.Parse(raiseInputText) + mostRecentBet;//raise value is equal to the players input + 2 times the most recent bet because to raise the bet it must be atleast two times the previous bet
            mostRecentBet = raiseValue;
            currentPlayer.mostRecentBet = mostRecentBet;//the pslayers mostRecentBet is set equal to the global mostRecentBet so that the players mostRecentBet is updated
            currentPlayer.numOfChips -= currentPlayer.mostRecentBet;//the players total number of chips has the raise value subtracted from it so that the players cumulative number of chips is updated
            mostRecentBetText.text = Convert.ToString(mostRecentBet);
            check_if_player_is_all_in();
            raise_calculations();
            add_chips_to_pot();
            increment_active_player();
            check_if_round_can_increment();
        }
    }
    //this updates the pot value text
    public void add_chips_to_pot()
    {
        print("addChipsToPot");
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        potValue += mostRecentBet;
        potValueText.text = Convert.ToString(potValue);
    }
    //placed raise calculations in function so broader function is easier to understand, raise calculations updates the players chips, only runs if player is not all in
    public void raise_calculations()
    {
        print("RaiseCalculations");
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        mostRecentBetText.text = Convert.ToString(mostRecentBet);
        currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
    }
    //allows the player to match the previous bet from their amount of chips and then incrementing the player by one
    public void call_on_click()
    {
        print("CallOnClick");
        raiseInput.text = "";
        errorMessageText.text = "";
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        currentPlayer.hasCalled = true;
        currentPlayer.mostRecentBet = mostRecentBet;//the players mostRecentBet is set equal to the previous global mostRecentBet
        currentPlayer.numOfChips -= currentPlayer.mostRecentBet;//the players cumulative amount of chips has their mostRecentBet subtracted from it
        mostRecentBetText.text = Convert.ToString(mostRecentBet);
        check_if_player_is_all_in();//chceks if player is going to go all in with their current bet they are trying, if they are then it updates values and skips to next player
        call_calculations();
        add_chips_to_pot();
        increment_active_player();
        check_if_round_can_increment();
    }
    //calculations updates the players chips
    public void call_calculations()
    {
        print("CallCalculations");
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        mostRecentBetText.text = Convert.ToString(mostRecentBet);
        currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
    }
    //folds the players hand and removes them from the bettiing for remainder of the hand
    public void fold_on_click()
    {
        print("FoldOnClick");
        raiseInput.text = "";
        errorMessageText.text = "";
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        currentPlayer.hasFolded = true;
        print(currentPlayer.hasFolded);
        debug_print("player" + activePlayerPosition + "has folded", currentPlayer.hasFolded);
        currentPlayer.gameObject.SetActive(false);
        increment_active_player();
        check_if_round_can_increment();
        check_all_folded();
    }
    //will add the amount of chips in the pot to the winning player determined by the evaluatehand function
    public void distribute_pot_at_hand_evaluation(List<GameObject> tempPlayers)//will be run when players cards are evaluated or everyone folds
    {
        print("DistributePotAtHandEvaluation");
        playerClassScript winningPlayer = tempPlayers[tempPlayers.Count-1].GetComponent<playerClassScript>();
        playerClassScript realWinningPlayer = get_player_by_id(winningPlayer.playerId);
        //real winning player is the real player version of tempPlayers, essentially players list without messing up rotations
        realWinningPlayer.numOfChips += potValue;//sets potValue to 0, sets numOfChipsInPot and adds to numOfChips on playerClassScript of player who won
        realWinningPlayer.playerChipsText.text = Convert.ToString(winningPlayer.numOfChips);
        handNumber++;
        round = 0;
    }
    //resets all the boolean values that are unimportant for the players identification, i.e. everything except player name and numOfChips
    public void start_next_hand()
    {
        print("startNextHand");
        //reset all boolean to orginal value
        allFolded = false;
        for (int i = 0; i < players.Count; i++)//for loop to loop through each player and set everything back to normal
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();

            currentPlayer.gameObject.SetActive(true);
            currentPlayer.hasFolded = false;
            currentPlayer.hasRaised = false;
            currentPlayer.hasCalled = false;
            currentPlayer.mostRecentBet = 0;
        }
        round = 0;
        potValue = 0;
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
        check_if_players_list_chips_valid();
        regenerate_players(players.Count, handNumber);
        check_if_players_list_chips_valid();
        deal_to_hands();
        round_tracker();
    }
    //this function regenerates the players with the new values required, i.e. a new little blind and big blind player is created, new original active player is defined
    public void regenerate_players(int numPlayers, int handNum)
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
        littleBlindPlayer.numOfChips -= littleBlindPlayer.mostRecentBet;
        littleBlindPlayer.playerChipsText.text = Convert.ToString(littleBlindPlayer.numOfChips);
        potValue += littleBlindPlayer.mostRecentBet;
        bigBlindPlayer.mostRecentBet = 10;
        bigBlindPlayer.numOfChips -= bigBlindPlayer.mostRecentBet;
        bigBlindPlayer.playerChipsText.text = Convert.ToString(bigBlindPlayer.numOfChips);
        potValue += bigBlindPlayer.mostRecentBet;
        mostRecentBet = bigBlindPlayer.mostRecentBet;
        potValueText.text = Convert.ToString(potValue);
        activePlayerPosition = (handNum + 2) % numPlayers;
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
    }
    public bool check_if_round_can_increment() //this function must be responsible for checking whether or not the round may increase, it must check whether or not the big-blind players bet is equal to the most recent bet (the little blinds bet), if it is not then the game will keep looping
    {
        print("checkIfroundCanIncrement");
        playerClassScript bigBlindPlayer = players[(1 + handNumber) % players.Count].GetComponent<playerClassScript>();
        if (bigBlindPlayer.hasCalled == true || bigBlindPlayer.hasFolded == true)
        {
            round += 1;
            round_tracker();
            bigBlindPlayer.hasCalled = false;//resets the values for bigBlind and littleBlind back to false
            return true;
        }
        else if (bigBlindPlayer.hasRaised == true)
        {
            return false;
        }
        return false;

    }
    //checks if every player except 1 is folded
    public void check_all_folded()
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
        if (count == players.Count - 1)
        {
            allFolded = true;
        }
        else
        {
            allFolded = false;
        }
        if (allFolded == true)
        {
            for (int i = 0; i < players.Count; i++)
            {
                playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
                if (currentPlayer.hasFolded == false)
                {
                    //uses the player found at position i for as the parameter as this is the winning player
                    check_if_temp_players_list_chips_valid(players, i);
                    distribute_pot_if_fold(i);
                }
            }
            check_if_game_should_end_players_list();
            start_next_hand();
        }
    }
    //distributes the pot to the player at position i found in the checkallfolded function
    public void distribute_pot_if_fold(int winningPlayerInt)
    {
        print("DistributePotAtHandEvaluation");
        playerClassScript winningPlayer = players[winningPlayerInt].GetComponent<playerClassScript>();
        winningPlayer.numOfChips += potValue;
        winningPlayer.playerChipsText.text = Convert.ToString(winningPlayer.numOfChips);
        handNumber++;
        round = 0;
    }
    //checks if the players bet will set them in debt, if it does sets the players to have gone all in
    public void check_if_player_is_all_in()
    {
        print("CheckIfPlayerGoesAllIn");
        playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
        if (currentPlayer.mostRecentBet >= currentPlayer.numOfChips)
        {
            //resets the bet to the total amount of chips in the player
            raiseValue = currentPlayer.numOfChips;
            currentPlayer.mostRecentBet = currentPlayer.numOfChips += mostRecentBet;
            mostRecentBet = currentPlayer.numOfChips;
            currentPlayer.numOfChips = 0;
            currentPlayer.isAllIn = true;
            currentPlayer.playerChipsText.text = Convert.ToString(currentPlayer.numOfChips);
        }
    }
    //checks if the temporary players list are allowed to keep playing, removes players that lose all in bets
    public void check_if_temp_players_list_chips_valid(List<GameObject> tempPlayers, int winningPlayerInt)
    {
        print("CheckIfPlayerIsAValid");
        for (int i = 0; i < tempPlayers.Count; i++)
        {
            playerClassScript currentPlayer = tempPlayers[i].GetComponent<playerClassScript>();
            if (currentPlayer.numOfChips <= 0 && i != winningPlayerInt)
            {
                tempPlayers.RemoveAt(i);
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
    //runs to check if players should if they eventually get 0 chips somewhow
    public void check_if_players_list_chips_valid()
    {
        print("CjeckifplayerChipbalanceisallowed");

        for (int i = 0; i < players.Count; i++)
        {
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            if (currentPlayer.numOfChips <= 0)
            {
                players.RemoveAt(i);
                currentPlayer.gameObject.SetActive(false);
            }
        }
    }
    //this checks if there is 1 player in the list of players and sends the game to the game over scene
    public void check_if_game_should_end_players_list()
    {
        print("CheckIfGameShouldEnd");
        //chekc if there is 1 player in the list
        if (players.Count == 1)
        {
            SceneManager.LoadScene(4);
        }
    }
    public void check_if_game_should_end_temp_players_list(List<GameObject> tempPlayers)
    {
        if (tempPlayers.Count == 1)
        {
            SceneManager.LoadScene(4);
        }
    }
    //this increments the active player to 1 position along the list
    public void increment_active_player()
    {
        print("IncrementActivePlayer");
        //CheckAllFolded();
        bool flag = true; //keeps track of loop running
        players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = false;
        while (flag)
        {
            activePlayerPosition = (activePlayerPosition + 1) % (players.Count);
            playerClassScript currentPlayer = players[activePlayerPosition].GetComponent<playerClassScript>();
            if (currentPlayer.hasFolded == false)
            {
                players[activePlayerPosition].gameObject.GetComponent<Image>().enabled = true;
                flag = false;//loop stops
            }
        }
    }
    //gets and sets each players hand value to a number, sorts the players hand and distributes the pot
    public void evaluate_hand()
    {
        print("EvaluateHand");
        for (int i = 0; i < players.Count; i++)
        {
            List<GameObject> handList = flopList.Concat(players[i].GetComponent<playerClassScript>().cards).ToList();//joins both flopList cards and the list of cards on the player
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            currentPlayer.valueOfCardsInHand = get_hand_rank(handList);
        }
        //players = sort_players_by_hand_rank();
        List<GameObject> tempPlayers =  sort_players_by_hand_rank();
        check_if_temp_players_list_chips_valid(tempPlayers, tempPlayers.Count - 1);
        //check_if_players_list_chips_valid();
        check_if_game_should_end_temp_players_list(tempPlayers);
        //check_if_game_should_end_players_list();
        distribute_pot_at_hand_evaluation(tempPlayers);
        //distribute_pot_at_hand_evaluation(players);
    }
    //sets each card face string as equivalent to a number, uses the card game object as a parameter
    public int get_face_power(GameObject currentCard)
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
    //compares hands in order of rank
    public int compare_hand_by_rank(GameObject player1, GameObject player2)
    {
        playerClassScript currentPlayer1 = player1.GetComponent<playerClassScript>();
        playerClassScript currentPlayer2 = player2.GetComponent<playerClassScript>();
        int player1Rank = currentPlayer1.valueOfCardsInHand;
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
    private int compare_face_by_power(GameObject card1, GameObject card2)
    {
        int face1Power = get_face_power(card1);
        int face2Power = get_face_power(card2);

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
    public List<GameObject> sort_hand_by_face_power(List<GameObject> handList)
    {
        handList.Sort(compare_face_by_power);
        return handList;
    }
    //makes new players list so it is sorted in order of hand rank so that the orginal players list is not ordered wrong
    public List<GameObject> sort_players_by_hand_rank()
    {
        List<GameObject> tempPlayers = new List<GameObject>(players);
        print("SortPlayersByHandRank");
        tempPlayers.Sort(compare_hand_by_rank);
        return tempPlayers;
    }
    //counts and returns the amount of a specific suit in a players hand
    public int get_number_of_suit_in_hand(List<GameObject> handList, string targetSuit)
    {

        int count = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            if (suit == targetSuit)
            {
                count += 1;
            }
        }
        return count;
    }
    //counts and returns the amount of a specific face in a players hand
    public int get_number_of_face_in_hand(List<GameObject> handList, string targetFace)
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
    public bool is_royal_flush(List<GameObject> handList)
    {
        if (get_number_of_face_in_hand(handList, "Ace") == 1 && get_number_of_face_in_hand(handList, "King") == 1 && get_number_of_face_in_hand(handList, "Queen") == 1 && get_number_of_face_in_hand(handList, "Jack") == 1 && get_number_of_face_in_hand(handList, "10") == 1 && get_number_of_suit_in_hand(handList, "Clubs") >= 5 || get_number_of_suit_in_hand(handList, "Diamonds") >= 5 || get_number_of_suit_in_hand(handList, "Spades") >= 5 || get_number_of_suit_in_hand(handList, "Hearts") >= 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
    public bool is_straight_flush(List<GameObject> handList)
    {

        for (int i = 0; i < handList.Count; i++)
        {
            int currentCount = 0;
            int currentFace = 0;
            string currentSuit = "";
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = get_face_power(handList[i]);
            currentSuit = suit;
            currentCount = 1;
            for (int n = i + 1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if ((get_face_power(handList[n]) == currentFace + 1
                || (nestedCardScript.face == "Ace"
                    && currentFace + 1 == 14)) && nestedCardScript.suit == currentSuit)
                {
                    currentCount += 1;
                    currentFace = get_face_power(handList[n]);
                    if (currentCount == 5)
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
    public bool is_four_of_a_kind(List<GameObject> handList)
    {
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = get_face_power(handList[i]);
            currentCount = 1;
            for (int n = i + 1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if (get_face_power(handList[n]) == currentFace)
                {
                    currentCount += 1;
                    if (currentCount == 4)
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
    public bool is_flush(List<GameObject> handList)
    {
        if (get_number_of_suit_in_hand(handList, "Diamonds") >= 5
  || get_number_of_suit_in_hand(handList, "Spades") >= 5
  || get_number_of_suit_in_hand(handList, "Clubs") >= 5
  || get_number_of_suit_in_hand(handList, "Hearts") >= 5
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool is_straight(List<GameObject> handList)
    {
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = get_face_power(handList[i]);
            currentCount = 1;
            for (int n = i + 1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if (get_face_power(handList[n]) == currentFace + 1
                || (nestedCardScript.face == "Ace"
                    && currentFace + 1 == 14))
                {
                    currentCount += 1;
                    currentFace = get_face_power(handList[n]);
                    if (currentCount == 5)
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
    public bool is_full_house(List<GameObject> handList)
    {
        string[] possibleFaces = new string[] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };
        string alreadyUsedFace = "";
        for (int i = 0; i < possibleFaces.Length; i++)
        {
            bool foundMultipleFaces = false;
            for (int n = 0; n < 13; n++)
            {
                if (alreadyUsedFace == "")
                {
                    if (get_number_of_face_in_hand(handList, possibleFaces[n]) >= 3)
                    {
                        alreadyUsedFace = possibleFaces[n];
                        foundMultipleFaces = true;
                        break;
                    }

                }
                else
                {
                    if (possibleFaces[n] != alreadyUsedFace && get_number_of_face_in_hand(handList, possibleFaces[n]) >= 2)
                    {
                        return true;
                    }
                }
            }
            if (!foundMultipleFaces)
            {
                alreadyUsedFace = "";
            }
        }
        return false;
    }
    //checks if the hand is of this type,returns true or false depending on whether or not the hand is this
    public bool is_three_of_a_kind(List<GameObject> handList)
    {
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = get_face_power(handList[i]);
            currentCount = 1;
            for (int n = i + 1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if (get_face_power(handList[n]) == currentFace)
                {
                    currentCount += 1;
                    if (currentCount == 3)
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
    public bool is_two_pair(List<GameObject> handList, string excludedFace)
    {

        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = get_face_power(handList[i]);

            if (face == excludedFace)
            {

                continue;
            }
            currentCount = 1;
            for (int n = i + 1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();

                if (get_face_power(handList[n]) == currentFace)
                {
                    currentCount += 1;
                    if (currentCount == 2)
                    {
                        if (excludedFace == "")
                        {

                            return is_two_pair(handList, nestedCardScript.face);//re runs function passing in the face we just found so it no longer searches for it
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
    public bool is_pair(List<GameObject> handList)
    {
        int currentCount = 0;
        int currentFace = 0;
        for (int i = 0; i < handList.Count; i++)
        {
            CardScript currentCardScript = handList[i].GetComponent<CardScript>();
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = get_face_power(handList[i]);
            currentCount = 1;
            for (int n = 0; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();

                if (get_face_power(handList[n]) == currentFace)
                {
                    currentCount += 1;
                    if (currentCount == 2)
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
    //checks if player has an ace king queen or jack
    public bool is_high_card(List<GameObject> handList)
    {
        if (get_number_of_face_in_hand(handList, "Ace") == 1 || get_number_of_face_in_hand(handList, "King") == 1 || get_number_of_face_in_hand(handList, "Queen") == 1 || get_number_of_face_in_hand(handList, "Jack") == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //this first sorts each players hand by its face rank and then returns an integer value
    public int get_hand_rank(List<GameObject> handList)
    {
        handList = sort_hand_by_face_power(handList);
        if (is_royal_flush(handList))
        {
            return 11;
        }
        else if (is_straight_flush(handList))
        {
            return 10;
        }
        else if (is_four_of_a_kind(handList))
        {
            return 9;
        }
        else if (is_full_house(handList))
        {
            return 8;
        }
        else if (is_flush(handList))
        {
            return 7;
        }
        else if (is_straight(handList))
        {
            return 6;
        }
        else if (is_three_of_a_kind(handList))
        {
            return 5;
        }
        else if (is_two_pair(handList, ""))
        {
            return 4;
        }
        else if (is_pair(handList))
        {
            return 3;
        }
        else if (is_high_card(handList))
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
}