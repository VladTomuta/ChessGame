using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericCard : MonoBehaviour
{

     // References
    private GameObject controller;

    public GameObject white_king, white_queen, white_knight, white_bishop, white_rook, white_pawn;
    public GameObject black_king, black_queen, black_knight, black_bishop, black_rook, black_pawn;

    private string player;
    private string cardName;
    private string cardDescription;
    private int cardCost;
    private Sprite cardImage;
    private Sprite cardBackgroundImage;
    

    void CreateCardFromPrefabs(string name) {
        switch (name) {
            case "white_pawn":
                
                break;
        }
    }











    

    // public GenericCard(string cardName, string cardDescription, int cardCost, Sprite cardImage, Sprite cardBackgroundImage) {
    //     this.cardName = cardName;
    //     this.cardDescription = cardDescription;
    //     this.cardCost = cardCost;
    //     this.cardImage = cardImage;
    //     this.cardBackgroundImage = cardBackgroundImage;
    // }

    public void SetCardName(string cardName) {
        this.cardName = cardName;
    }

    public void SetCardDescription(string cardDescription) {
        this.cardDescription = cardDescription;
    }

    public void SetCardCost(int cardCost) {
        this.cardCost = cardCost;
    }

    public void SetCardImage(Sprite cardImage) {
        this.cardImage = cardImage;
    }

    public void SetCardBackgroundImage(Sprite cardBackgroundImage) {
        this.cardBackgroundImage = cardBackgroundImage;
    }

    public string GetCardName() {
        return cardName;
    } 

    public string GetCardDescription() {
        return cardDescription;
    }

    public int GetCardCost() {
        return cardCost;
    }

    public Sprite GetCardImage() {
        return cardImage;
    }

    public Sprite GetCardBackgroundImage() {
        return cardBackgroundImage;
    }
}
