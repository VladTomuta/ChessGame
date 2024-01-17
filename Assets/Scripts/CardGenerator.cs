using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    [SerializeField] public GameObject whitePawn, whiteRook, whiteKnight, whiteBishop, whiteQueen;
    [SerializeField] public GameObject blackPawn, blackRook, blackKnight, blackBishop, blackQueen;

    public void CreateCard(string cardName, GameObject playerHand) {

        GameObject newCard;


        switch (cardName) {
            case "white_pawn":
                newCard = Instantiate(whitePawn, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
            case "white_rook":
                newCard = Instantiate(whiteRook, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
            case "white_knight":
                newCard = Instantiate(whiteKnight, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
            case "white_bishop":
                newCard = Instantiate(whiteBishop, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
            case "white_queen":
                newCard = Instantiate(whiteQueen, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
            case "black_pawn":
                newCard = Instantiate(blackPawn, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
            case "black_rook":
                newCard = Instantiate(blackRook, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
            case "black_knight":
                newCard = Instantiate(blackKnight, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
            case "black_bishop":
                newCard = Instantiate(blackBishop, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
            case "black_queen":
                newCard = Instantiate(blackQueen, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.transform.parent = playerHand.transform;
                newCard.transform.localScale = Vector3.one;
                break;
        }

    }
}
