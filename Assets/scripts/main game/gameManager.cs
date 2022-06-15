using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class gameManager : MonoBehaviour
{
    //defining variables used in game
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
    public List<GameObject> flopList;
    public GameObject flopGrid;
    public List<GameObject> playerPositions;
    public bool debug = false;


    //runs on start, generates players, deck, shuffles deck, places the players around the table and deals them cards
    void Start()
    {
        generate_players(PlayerPrefs.GetInt("players"), 1);
        deckScript.generate();
        deckScript.shuffle();
        generate_player_objects_around_table();
        deal_to_hands();
    }


    //GENERIC FUNCTIONS

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


    //ROUND INCREMENTATION IS CONTROLLED HERE ASWELL AS WHAT HAPPENS WHEN ROUNDS INCREMENT, i.e. DEAL TO FLOP and DEAL TO CARD

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
            start_next_hand();

        }
    }
    public bool check_if_round_can_increment() //this function checks if the big blind player folds are calls the most recent bet, if they do either the round can increment, if not everyone will keep playing
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


    //INITIAL GENERATION OF PLAYERS, ONLY RUNS AT START

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
    


    //THESE INSTANTIATE PREFABS AT DIFFERENT POSITIONS, PLAYERS AT TABLE ,DEAL CARDS TO PLAYERS AND THE TABLE

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


    //BUTTONS, THESE RUN WHEN PLAYERS CLICK THE BUTTONS, CONTROLS THE ROTATION OF PLAYERS,CALL OTHER FUNCTIONS LIKE CHECKALLFOLDED

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



    //DISTRIBUTION OF CHIPS, THESE RUN WHEN BEFORE A NEW HAND BEGINS


    //gets and sets each players hand value to a number, sorts the players hand and distributes the pot to the player who will win
    public void evaluate_hand()
    {
        print("EvaluateHand");
        for (int i = 0; i < players.Count; i++)
        {
            List<GameObject> handList = flopList.Concat(players[i].GetComponent<playerClassScript>().cards).ToList();//joins both flopList cards and the list of cards on the player
            playerClassScript currentPlayer = players[i].GetComponent<playerClassScript>();
            currentPlayer.valueOfCardsInHand = get_hand_rank(handList);
        }
        List<GameObject> tempPlayers = sort_players_by_hand_rank();
        check_if_temp_players_list_chips_valid(tempPlayers, tempPlayers.Count - 1);//checks if the players should be removed if they failed an all in bet, uses temp players so the rotation is not bugged
        check_if_game_should_end_temp_players_list(tempPlayers);//checks if the temporary players list is of  1 player
        distribute_pot_at_hand_evaluation(tempPlayers);//distributes the pot at to the winning player, the winning player is determined by the temporary list as not to screw up the rotation in later hands, the winning player is then converted back into the real players position or players list using a unique playerId given to each player
    }
    //will add the amount of chips in the pot to the winning player determined by the evaluatehand function
    public void distribute_pot_at_hand_evaluation(List<GameObject> tempPlayers)//will be run when players cards are evaluated at round 4
    {
        print("DistributePotAtHandEvaluation");
        playerClassScript winningPlayer = tempPlayers[tempPlayers.Count-1].GetComponent<playerClassScript>();
        //real winning player is the real player version of tempPlayers, essentially players list without messing up rotations
        playerClassScript realWinningPlayer = get_player_by_id(winningPlayer.playerId);
        realWinningPlayer.numOfChips += potValue;//adds to numOfChips on playerClassScript of player who won
        realWinningPlayer.playerChipsText.text = Convert.ToString(winningPlayer.numOfChips);
        handNumber++;
        round = 0;
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


    //FUNCTIONS THAT RUN WHEN A NEW HAND WILL START, AFTER A PLAYER WINS

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
        deckScript.generate();
        deckScript.shuffle();
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
    

    //THESE FUNCTIONS CHECK FOR CERTAIN THINGS, i.e. IF EVERYONE IS FOLDED, IF A PLAYER WILL GO ALL IN WITH THEIR BETS, IF A PLAYER NEEDS TO BE REMOVED FROM THE GAME

    
    //checks if every player except 1 is folded, determines the remaining player as the player that will be given chips at the distribution
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
                //moves player index down 1
                i--;
                //moves the winning player down 1 to compensate for this as players are removed at i and the list length decreases by one
                if (i < winningPlayerInt)
                {
                    winningPlayerInt--;
                }
            }
        }
    }
    //runs to check if players should be removed if they eventually get 0 chips
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
    

    //PLAYER INCREMENTATION, WHICH PLAYER IS ACTING
    
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
    

    //GETTING CARD FACE AND SUIT VALUES, i.e. NUMBER OF A CERTAIN CARD FACE OR SUIT IN A PLAYERS HAND

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


    //SORTING OF HANDS AND PLAYERS, PLAYERS ARE SORTED WHEN THE POT NEEDS TO BE DISTRIBUTED, HANDS/CARDS IN THE HAND ARE SORTED SO THE BOOLEAN FUNCTIONS BELOW CAN WORK AS REQUIRED (THEY RELY ON HANDS TO BE SORTED IN ORDER)

    //compares two players hands in order of rank
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
    //compares two cards in order of power
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
        handList.Sort(compare_face_by_power);//sorts hand using compare face by power so the straight functions can work smoother
        return handList;
    }
    

    //IMPORTANT
    //makes new temporary players list that is sorted in order of hand rank so that the orginal players list is not ordered wrong and does not ruin rotation
    public List<GameObject> sort_players_by_hand_rank()
    {
        //temp players is a clone of the players list so calculations can be done to it avoiding bugs regarding list sorting and rotation (if the players list was sorted it would be permanently in that order and would need to be "unsorted" for rotation to be accurate)
        List<GameObject> tempPlayers = new List<GameObject>(players);
        print("SortPlayersByHandRank");
        tempPlayers.Sort(compare_hand_by_rank);
        return tempPlayers;
    }
    //gets the players id for each player so it can be used to reference their actuall information in the pot distribution to avoid the sorting of players list, this knows where each player is, the playerId parameter is the position i in the for loop as it loops through in generates players
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
    //EXPLANATION OF THE TEMPORARY PLAYERS LIST, 
    //the temporary players list was made because of a bug resulting from the players being sorted, after the players were sorted they would be in that order for the entire next hand and so the rotation
    //of hands would mess up (go to the wrong spot)
    //the temp players list is a cloned list of players which allows for sorting and distributing without interacting with the rotation of players
    //this required a playerId to be created to keep track of where the tempPlayers corresponded to the actual player, and so the playerId is used to reference the players in the playersList for calculations


    //CHECKING IF A HAND LIST CONTAINS A CERTAIN RANK OF HAND

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
            for (int n = i + 1; n < handList.Count; n++)//loops through handList beginning 1 item after current card
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();//nested card is the card 1 over from currentCard, does not need to check for currentCard again
                if (get_face_power(handList[n]) == currentFace)//checks if the power of the face incremented 1 across is equal to the power of the face before it
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
            //getting the cards suit and face string and storing it
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = get_face_power(handList[i]);
            currentCount = 1;
            for (int n = i + 1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if (get_face_power(handList[n]) == currentFace + 1//this is checking if the integer value of the face found is +1 from the previous
                || (nestedCardScript.face == "Ace"
                    && currentFace + 1 == 14))//handles the circumstance when the ace is needed to make a straight, it must  be both a 1 and 14, this is the circumstance in which it must be 14
                {
                    currentCount += 1;
                    currentFace = get_face_power(handList[n]);
                    if (currentCount == 5)//if it finds 5 cards of increasing value it returns true for a straight
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
    //checks if the hand contains 3 cards of 1 face and 2 other cards of the same face
    public bool is_full_house(List<GameObject> handList)
    {
        string[] possibleFaces = new string[] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };//defining the possible faces the hand could be made of
        string alreadyUsedFace = "";//defines a string to store the face that is found
        for (int i = 0; i < possibleFaces.Length; i++)//loops through the possible faces
        {
            bool foundMultipleFaces = false;
            for (int n = 0; n < 13; n++)
            {
                if (alreadyUsedFace == "")
                {
                    if (get_number_of_face_in_hand(handList, possibleFaces[n]) >= 3)//checks if the type of face at index n is greater than 3
                    {
                        alreadyUsedFace = possibleFaces[n];//excludes the 3 cards found from the remaining search
                        foundMultipleFaces = true;
                        break;
                    }

                }
                else
                {
                    if (possibleFaces[n] != alreadyUsedFace && get_number_of_face_in_hand(handList, possibleFaces[n]) >= 2)//if it finds 2 cards with the same face and it is not already found
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
            //getting the cards suit and face string and storing it
            string suit = currentCardScript.suit;
            string face = currentCardScript.face;
            currentFace = get_face_power(handList[i]);
            currentCount = 1;
            for (int n = i + 1; n < handList.Count; n++)
            {
                CardScript nestedCardScript = handList[n].GetComponent<CardScript>();
                if (get_face_power(handList[n]) == currentFace)//checks is two cards match in face power
                {
                    currentCount += 1;//loops through and counts how many of the same face there are, if there is 3 it returns true, if not it continues searching
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
            //getting the cards suit and face string and storing it
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

                if (get_face_power(handList[n]) == currentFace)//checks if the two cards match in power
                {
                    currentCount += 1;//increases the count of pairs found
                    if (currentCount == 2)
                    {
                        if (excludedFace == "")//if the pair found has not already been found
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
    //checks if player has 1 ace king queen or jack
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
    
    
    //this first sorts each players hand by its face rank and then returns an integer value which can be used to set players hand values
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