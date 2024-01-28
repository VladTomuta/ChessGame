using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    [SerializeField] Sprite attackMovePlate; 

    GameObject reference = null;

    //Board positions, not world positions
    int matrixX;
    int matrixY;

    //false: movement, true: attacking
    private bool attack = false;

    private GameObject castlingRook = null;

    public void Start() {
        if (attack) {
            //Change to red
            //gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            gameObject.GetComponent<SpriteRenderer>().sprite = attackMovePlate;
            gameObject.GetComponent<Transform>().localScale = new Vector3(4.7f, 4.7f, gameObject.GetComponent<Transform>().localScale.z);
        }
    }

    public void OnMouseUp() {
        Chessman chessmanScript = reference.GetComponent<Chessman>();
        chessmanScript.DestroyMovePlates("MovePlate");
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game gameScript = controller.GetComponent<Game>();
        
        gameScript.SetGameHasStarted(true);

        chessmanScript.DestroyAllLastMovesServerRpc("LastMove");
        chessmanScript.SpawnLastMoveServerRpc(chessmanScript.GetXBoard(), chessmanScript.GetYBoard());
        chessmanScript.SpawnLastMoveServerRpc(matrixX, matrixY);

        if(attack) {
            GameObject chessPiece;

            if(gameScript.GetPosition(matrixX, matrixY)) {
                chessPiece = gameScript.GetPosition(matrixX, matrixY);
                if (chessPiece.name == "white_king") 
                    //StartCoroutine(WinnerServerRpcWithDelay("black", "capture", 0.5f));
                    gameScript.WinnerServerRpc("black", "capture");
                else if (chessPiece.name == "black_king")
                    //StartCoroutine(WinnerServerRpcWithDelay("white", "capture", 0.5f));
                    gameScript.WinnerServerRpc("white", "capture");
                else if (CheckIfOnlyKingsAreLeft()) 
                    //StartCoroutine(WinnerServerRpcWithDelay("tie", "onlyKingsLeft", 0.5f));
                    gameScript.WinnerServerRpc("tie", "onlyKingsLeft");
            } else {
                if (gameScript.GetCurrentPlayer() == "white") {
                    chessPiece = gameScript.GetPosition(matrixX, matrixY - 1);
                } else {
                    chessPiece = gameScript.GetPosition(matrixX, matrixY + 1);
                }
            }

            

            gameScript.DestroyPieceServerRpc(chessPiece);
        }

        gameScript.SetPositionEmptyServerRpc(chessmanScript.GetXBoard(),
                                             chessmanScript.GetYBoard());

        gameScript.SetRefToPossibleEnPassantPawnServerRpc(reference.GetComponent<NetworkObject>(), true);

        if(Math.Abs(chessmanScript.GetYBoard() - matrixY) == 2 && (reference.name == "white_pawn" || reference.name == "black_pawn")) {
            gameScript.SetRefToPossibleEnPassantPawnServerRpc(reference.GetComponent<NetworkObject>(), false);
        }

        //DISABLED UNTIL MULTIPLAYER WORKS
        if(reference.name == "white_pawn" && matrixY == 7) {
            gameScript.DestroyPieceServerRpc(reference.GetComponent<NetworkObject>());
            //Destroy(reference);
            //reference = gameScript.Create("white_queen", matrixX, matrixY).Value;
            gameScript.CreatePieceServerRpc("white_queen", matrixX, matrixY);
            //reference = networkVariable.Value;

        } else if (reference.name == "black_pawn" && matrixY == 0) {
            // Destroy(reference);
            // reference = gameScript.Create("black_queen", matrixX, matrixY).Value;
            gameScript.DestroyPieceServerRpc(reference.GetComponent<NetworkObject>());
            gameScript.CreatePieceServerRpc("black_queen", matrixX, matrixY);
            //Destroy(reference);
            //reference = gameScript.Create("white_queen", matrixX, matrixY).Value;
            //gameScript.CreateServerRpc(ref reference, "black_queen", matrixX, matrixY);
            //reference = networkVariable.Value;
        } else {
            chessmanScript.SetXBoardServerRpc(matrixX);
            chessmanScript.SetYBoardServerRpc(matrixY);
            chessmanScript.SetCoordsServerRpc();
            chessmanScript.SetHasMovedServerRpc(reference.GetComponent<NetworkObject>() ,true);

            gameScript.SetPositionServerRpc(reference.GetComponent<NetworkObject>());
        }

        if(castlingRook) {
            Chessman rookChessmanScript = castlingRook.GetComponent<Chessman>();

            gameScript.SetPositionEmptyServerRpc(rookChessmanScript.GetXBoard(),
                                                 rookChessmanScript.GetYBoard());
                                        
            if(rookChessmanScript.GetXBoard() == 0) {
                rookChessmanScript.SetXBoardServerRpc(3);
            } else {
                rookChessmanScript.SetXBoardServerRpc(5);
            }

            rookChessmanScript.SetYBoardServerRpc(rookChessmanScript.GetYBoard());
            rookChessmanScript.SetCoordsServerRpc();
            rookChessmanScript.SetHasMovedServerRpc(reference.GetComponent<NetworkObject>(), true);
            
            gameScript.SetPositionServerRpc(castlingRook.GetComponent<NetworkObject>());
        }

        gameScript.NextTurn();
    }

    public void SetCoords(int x, int y) {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj) {
        reference = obj;
    }

    public void SetAttack(bool attack) {
        this.attack = attack;
    }

    public void SetCastling(GameObject castling) {
        this.castlingRook = castling;
    }

    public GameObject GetReference() {
        return reference;
    }

    public bool GetAttack() {
        return attack;
    }

    public bool CheckIfOnlyKingsAreLeft() {
        GameObject[] chessPieces = GameObject.FindGameObjectsWithTag("ChessPiece");

        if (AreThereCardsLeft())
            return false;

        Debug.Log("There are cards left");
        Debug.Log("chessPieces.Length == " + chessPieces.Length);

        if (chessPieces.Length != 3) 
            return false;

        Debug.Log("There are more then 2 pieces left");

        return true;
    }

    public bool AreThereCardsLeft()
    {
        GameObject canvas = GameObject.Find("Canvas");

        Transform whiteCards = canvas.transform.Find("WhiteCards");
        Transform blackCards = canvas.transform.Find("BlackCards");

        if (whiteCards.childCount > 0)
        {
            return true;
        }

        if (blackCards.childCount > 0)
        {
            return true;
        }

        return false;
    }

    IEnumerator WinnerServerRpcWithDelay(string playerWinner, string winCondition, float delay)
    {
        Debug.Log("Started delay");
        yield return new WaitForSeconds(delay);
        Game gameScript = controller.GetComponent<Game>();

        Debug.Log("End delay");
        gameScript.WinnerServerRpc(playerWinner, winCondition);

        
    }
}
