using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DeckScript : MonoBehaviour
{
    
    //defines the card prefab and cardgroup so that the deck may be displayed visually
    public GameObject cardPrefab;
    public Transform cardGroup;
    public Transform cardGroupDealtToFlop;
    //defines the list of cards as game objects
    public List<GameObject> cards;
    //defines all the required values of each card as string arrays
    string[] face = new string[] { "Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King" };
    string[] suit = new string[] { "Clubs", "Diamonds", "Hearts", "Spades" };
    string[] iconSuit = new string[] { "♣", "♦", "♥", "♠" };
    string[] iconFace = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    
   
   
    //Generate() destroys and clears each game object in th list prior to generating the new list and card prefabs
    public void Generate()
    {
        foreach (Transform item in cardGroup.transform)
        {
            Destroy(item.gameObject);
        }
        cards.Clear();
        //each for loop cycles through the length of both face and suit to instantiate the values onto the card prefabs
        for (int i = 0; i < face.Length; i++)
        {
            for (int f = 0; f < suit.Length; f++)
            {
                GameObject newCard = Instantiate(cardPrefab);
                newCard.GetComponent<CardScript>().UpdateCard(face[i],suit[f],iconFace[i],iconSuit[f]);
                cards.Add(newCard);
                //sets the new cards to children of the cardgroup
                newCard.transform.SetParent(cardGroup);
                //print(face[i] +" of "+suit[f]);
            }
        }
    }
    //shuffle removes the cards from the list, randomises there positions and re instates them in the list in their new positions
    public void Shuffle()
    {
        for (int i = 0; i < 52; i++)
        {
            cards[i].transform.SetParent(transform);
        }
        for (int i = 0; i < 10000; i++)
        {

            int rnd = UnityEngine.Random.Range(0, 52);
            int rnd1 = UnityEngine.Random.Range(0, 52);

            GameObject temp = cards[rnd];
            cards[rnd] = cards[rnd1];
            cards[rnd1] = temp;
        }
        for (int i = 0; i < 52; i++)
        {
            cards[i].transform.SetParent(cardGroup);
        }

        //re add children to parent
    }
    

}


