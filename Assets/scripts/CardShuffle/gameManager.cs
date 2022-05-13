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
    public int littleBlindBetValue;
    public int bigBlindBetValue;
    public int potValue;
    public int callValue;
    public int raiseValue;
    public int roundNum;
    public bool allFolded = false;
    public bool potOver = false;
    public List<GameObject> players;
    public GameObject playerPrefab;
    public GameObject cardGroups;
    public List<GameObject> flopList;
    public GameObject flopGrid;
    public List<GameObject> playerPositions;
    
    //make bool checking if playerName  so each player can see what their cards is but not what others are
    void Start()
    {
        deckScript.Generate();
        deckScript.Shuffle();
        DealToFlop1();
        GeneratePlayers(3);
        GeneratePlayerObjects();
        DealToHands();
    }
    //gameLoop (while)
    public void gameLoop()
    {
        //define round =1
        //define player=1
        //make function checking what player is active
        //make function that asks for fold, raise and call
        //each function checks what player is active and increments the player to the next one based on if they act
        //once it has gone through each player, set player back to 1 and increment round by one
        //repeat for all rounds
        roundNum = 0;
        
        /*while(roundNum == 0 && allFolded == false)//while game is running, ends on conditions of everyone has folded, the 4th round ends
        {
            //must determine the players hand values, determines the who wins the pot
            for (int i = 0; i < players.Count; i++)//loops for each player
            {
                //conditionally checks if each player has acted, using hasCalled, hasFolded, hasRaised,
                if(raiseButton has been clicked or callButton has been clicked or foldButton has been clicked)
                {
                    //then set for the player in the list their respective hasActed bool to true and move to next playr in list
                }
            }
            roundNum++;
        }
        while(roundNum==1 && allFolded == false)
        {
            for (int i = 0; i < players.Count; i++)
            {

            }
            roundNum++;
        }
        while(roundNum == 2&& allFolded == false)
        {
            for (int i = 0; i < players.Count; i++)
            {

            }
            roundNum++;
        }
        while(roundNum == 3 && allFolded == false)
        {
            for (int i = 0; i < players.Count; i++)
            {

            }
            potOver = true;
        }*/
    }
    
    public void DetermineLittleBlind()
    {

    }
    public void DetermineBigBlind()
    {
        bigBlindBetValue = 2 * littleBlindBetValue;
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
    //make pre-flop round, ask for bets and actions,
    public void DealToFlop1()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
            deckScript.cards.Remove(cardToMove); //removes card from list
            flopList.Add(cardToMove);
            cardToMove.transform.SetParent(flopGrid.transform);
        }
    }
    public void DealToFlop2()
    {
        GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
        deckScript.cards.Remove(cardToMove); //removes card from list
        flopList.Add(cardToMove);
        cardToMove.transform.SetParent(flopGrid.transform);
    }
    public void raiseButtonOnClick()
    {
        //runs raise function
    }
    public void callButtonOnClick()
    {
        //runs call function
    }
    public void foldButtonOnClick()
    {
        //runs fold function
    }

    

}
