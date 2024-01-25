using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardGenerator : NetworkBehaviour
{
    [SerializeField] public GameObject whitePawn, whiteRook, whiteKnight, whiteBishop, whiteQueen;
    [SerializeField] public GameObject blackPawn, blackRook, blackKnight, blackBishop, blackQueen;

    public void CreateCard(string cardName, GameObject playerHand, string player) {

        GameObject newCard;

        switch (cardName) {
            case "white_pawn":
                newCard = Instantiate(whitePawn, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                // newCard.transform.parent = playerHand.transform;
                // newCard.transform.localScale = Vector3.one;
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
            case "white_rook":
                newCard = Instantiate(whiteRook, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                // newCard.transform.parent = playerHand.transform;
                // newCard.transform.localScale = Vector3.one;
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
            case "white_knight":
                newCard = Instantiate(whiteKnight, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                // newCard.transform.parent = playerHand.transform;
                // newCard.transform.localScale = Vector3.one;
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
            case "white_bishop":
                newCard = Instantiate(whiteBishop, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                // newCard.transform.parent = playerHand.transform;
                // newCard.transform.localScale = Vector3.one;
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
            case "white_queen":
                newCard = Instantiate(whiteQueen, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
            case "black_pawn":
                newCard = Instantiate(blackPawn, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
            case "black_rook":
                newCard = Instantiate(blackRook, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
            case "black_knight":
                newCard = Instantiate(blackKnight, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
            case "black_bishop":
                newCard = Instantiate(blackBishop, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
            case "black_queen":
                newCard = Instantiate(blackQueen, new Vector3(0, 0, -1), Quaternion.identity);
                newCard.GetComponent<NetworkObject>().Spawn(true);
                SetCardDataClientRpc(newCard, playerHand, player);
                break;
        }      
    }

    [ClientRpc]
    public void SetCardDataClientRpc(NetworkObjectReference cardReference, NetworkObjectReference playerHandReference, string player) {
        cardReference.TryGet(out NetworkObject card);
        playerHandReference.TryGet(out NetworkObject playerHand);
        card.transform.parent = playerHand.transform;
        card.transform.localScale = Vector3.one;
        card.name = card.name.Replace("(Clone)", "");
        card.GetComponent<CardHandler>().SetPlayer(player);
    }
}
