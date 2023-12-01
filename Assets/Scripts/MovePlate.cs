using System;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

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
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp() {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game gameScript = controller.GetComponent<Game>();
        Chessman chessmanScript = reference.GetComponent<Chessman>();

        if(attack) {
            GameObject chessPiece;

            if(gameScript.GetPosition(matrixX, matrixY)) {
                chessPiece = gameScript.GetPosition(matrixX, matrixY);
                if (chessPiece.name == "white_king") gameScript.Winner("black");
                if (chessPiece.name == "black_king") gameScript.Winner("white");
            } else {
                if (gameScript.GetCurrentPlayer() == "white") {
                    chessPiece = gameScript.GetPosition(matrixX, matrixY - 1);
                } else {
                    chessPiece = gameScript.GetPosition(matrixX, matrixY + 1);
                }
            }

            Destroy(chessPiece);
        }

        gameScript.SetPositionEmpty(chessmanScript.GetXBoard(),
                                    chessmanScript.GetYBoard());

        gameScript.SetRefToPossibleEnPassantPawn(null);

        if(Math.Abs(chessmanScript.GetYBoard() - matrixY) == 2 && (reference.name == "white_pawn" || reference.name == "black_pawn")) {
            gameScript.SetRefToPossibleEnPassantPawn(reference);
        }
        
        if(reference.name == "white_pawn" && matrixY == 7) {
            Destroy(reference);
            reference = gameScript.Create("white_queen", matrixX, matrixY);

        } else if (reference.name == "black_pawn" && matrixY == 0) {
            Destroy(reference);
            reference = gameScript.Create("black_queen", matrixX, matrixY);
        }

        chessmanScript.SetXBoard(matrixX);
        chessmanScript.SetYBoard(matrixY);
        chessmanScript.SetCoords();
        chessmanScript.SetHasMoved(true);

        gameScript.SetPosition(reference);

        if(castlingRook) {
            Chessman rookChessmanScript = castlingRook.GetComponent<Chessman>();

            gameScript.SetPositionEmpty(rookChessmanScript.GetXBoard(),
                                        rookChessmanScript.GetYBoard());
                                        
            if(rookChessmanScript.GetXBoard() == 0) {
                rookChessmanScript.SetXBoard(3);
            } else {
                rookChessmanScript.SetXBoard(5);
            }

            rookChessmanScript.SetYBoard(rookChessmanScript.GetYBoard());
            rookChessmanScript.SetCoords();
            rookChessmanScript.SetHasMoved(true);
            
            gameScript.SetPosition(castlingRook);
        }

        gameScript.NextTurn();

        chessmanScript.DestroyMovePlates();
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
}
