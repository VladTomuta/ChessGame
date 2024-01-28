using System;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlate : MonoBehaviour
{
    public Game gameScript;

    CardHandler reference = null;

    //Board positions, not world positions
    int matrixX;
    int matrixY;

    public void OnMouseUp() {
        reference.DestroyMovePlates("MovePlate");

        Game gameScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        gameScript.SetGameHasStarted(true);

        gameScript.CreatePieceServerRpc(GetPieceNameFromCardName(reference.name), matrixX, matrixY);
        reference.DestroyAllLastMovesServerRpc("LastMove");
        reference.SpawnLastMoveServerRpc(matrixX, matrixY);
        
        gameScript.DestroyCardServerRpc(reference.GetComponent<NetworkObject>());
        gameScript.NextTurn();
    }

    public void SetCoords(int x, int y) {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj) {
        reference = obj.GetComponent<CardHandler>();
    }

    public CardHandler GetReference() {
        return reference;
    }

    public string GetPieceNameFromCardName(string cardName) {

        string pieceName = null;

        switch (cardName) {
            case "WhitePawnCard":
                pieceName = "white_pawn";
                break;
            case "WhiteRookCard":
                pieceName = "white_rook";
                break;
            case "WhiteKnightCard":
                pieceName = "white_knight";
                break;
            case "WhiteBishopCard":
                pieceName = "white_bishop";
                break;
            case "WhiteQueenCard":
                pieceName = "white_queen";
                break;
            case "BlackPawnCard":
                pieceName = "black_pawn";
                break;
            case "BlackRookCard":
                pieceName = "black_rook";
                break;
            case "BlackKnightCard":
                pieceName = "black_knight";
                break;
            case "BlackBishopCard":
                pieceName = "black_bishop";
                break;
            case "BlackQueenCard":
                pieceName = "black_queen";
                break;
        }

        return pieceName;
    }
}
