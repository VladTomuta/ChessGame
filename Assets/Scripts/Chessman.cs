using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Chessman : NetworkBehaviour
{
    // References
    private GameObject controller;
    public GameObject movePlate;

    // Positions
    private int xBoard = -1;
    private int yBoard = -1;

    // Variable ot keep track of "black" player or "white" player
    private string player;

    // Variable to keep track if the piece has moved or not this game
    private bool hasMoved;
    private bool canBeCapturedEnPassant;

    // References for all the sprites that the chesspiece can be
    public Sprite black_king, black_queen, black_knight, black_bishop, black_rook, black_pawn;
    public Sprite white_king, white_queen, white_knight, white_bishop, white_rook, white_pawn;

    void Start() {
        Activate();
    }

    public void Activate() {
        controller = GameObject.FindGameObjectWithTag("GameController");

        //take the instantiated location and adjust the transform
        SetCoordsServerRpc();

        switch (this.name) {
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black";break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;

            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCoordsServerRpc() {
        float x = xBoard;
        float y = yBoard;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard() {
        return xBoard;
    }

    public int GetYBoard() {
        return yBoard;
    }

    public bool GetHasMoved() {
        return hasMoved;
    }

    public bool GetCanBeCapturedEnPassent() {
        return canBeCapturedEnPassant;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetXBoardServerRpc(int x) {
        xBoard = x;
        SetXBoardClientRpc(x);
    }

    [ClientRpc]
    public void SetXBoardClientRpc(int x) {
        xBoard = x;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetYBoardServerRpc(int y) {
        yBoard = y;
        SetYBoardClientRpc(y);
    }

    [ClientRpc]
    public void SetYBoardClientRpc(int y) {
        yBoard = y;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetHasMovedServerRpc(NetworkObjectReference reference, bool hasMoved) {
        SetHasMovedClientRpc(reference, hasMoved);
    }

    [ClientRpc]
    public void SetHasMovedClientRpc(NetworkObjectReference reference, bool hasMoved) {
        reference.TryGet(out NetworkObject obj);
        Debug.Log(obj);
        Chessman chessmanScript = obj.GetComponent<Chessman>();
        chessmanScript.hasMoved = hasMoved;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetCanBeCapturedEnPassentServerRpc(bool canBeCapturedEnPassant) {
        this.canBeCapturedEnPassant = canBeCapturedEnPassant;
    }

    private void OnMouseUp() {

        if(controller.GetComponent<Game>().GetCurrentPlayer() == "white" && !IsHost) {
            return;
        } else if(controller.GetComponent<Game>().GetCurrentPlayer() == "black" && IsHost){
            return;
        }

        Debug.Log(controller.GetComponent<Game>().IsGameOver());
        Debug.Log(controller.GetComponent<Game>().GetCurrentPlayer());
        Debug.Log(player);
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player) {
            Debug.Log("Alo ne spawnam si noi?");
            DestroyMovePlates();
            InitiateMovePlates();
        } else {
            Debug.Log("Nu ne trezim");
        }
    }

    public void DestroyMovePlates() {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++) {
            Destroy(movePlates[i]);
        }
    }

    public void InitiateMovePlates() {
        Debug.Log(this.name);
        switch(this.name) {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1, 0);   //right
                LineMovePlate(0, 1);   //up
                LineMovePlate(1, 1);   //up-right
                LineMovePlate(-1, 0);  //left
                LineMovePlate(0, -1);  //down
                LineMovePlate(-1, -1); //down-left
                LineMovePlate(-1, 1);  //up-left
                LineMovePlate(1, -1);  //down-right
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);   //up-right
                LineMovePlate(-1, -1); //down-left
                LineMovePlate(-1, 1);  //up-left
                LineMovePlate(1, -1);  //down-right
                break;
            case "black_king":
            case "white_king":
                KingMovePlate();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);   //right
                LineMovePlate(0, 1);   //up
                LineMovePlate(-1, 0);  //left
                LineMovePlate(0, -1);  //down
                break;
            case "black_pawn":
                BlackPawnMovePlate(xBoard, yBoard - 1);
                break;
            case "white_pawn":
                WhitePawnMovePlate(xBoard, yBoard + 1);
                break;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement) {
        Game gameScript = controller.GetComponent<Game>();
        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while(gameScript.PositionOnBoard(x, y) && gameScript.GetPosition(x, y) == null) {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (gameScript.PositionOnBoard(x, y) && gameScript.GetPosition(x, y).GetComponent<Chessman>().player != player) {
            MovePlateAttackSpawn(x, y);
        }
    }

    public void LMovePlate() {
        PointMovePlate(xBoard + 1, yBoard  + 2);
        PointMovePlate(xBoard - 1, yBoard  + 2);
        PointMovePlate(xBoard + 2, yBoard  + 1);
        PointMovePlate(xBoard + 2, yBoard  - 1);
        PointMovePlate(xBoard + 1, yBoard  - 2);
        PointMovePlate(xBoard - 1, yBoard  - 2);
        PointMovePlate(xBoard - 2, yBoard  + 1);
        PointMovePlate(xBoard - 2, yBoard  - 1);
    }

    public void KingMovePlate() {
        PointMovePlate(xBoard, yBoard  + 1);
        PointMovePlate(xBoard, yBoard  - 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard  + 1);
        PointMovePlate(xBoard + 1, yBoard  - 1);
        PointMovePlate(xBoard - 1, yBoard  + 1);
        PointMovePlate(xBoard - 1, yBoard  - 1);

        Game gameScript = controller.GetComponent<Game>();
        
        if(gameScript.GetCurrentPlayer() == "white") {
            GameObject leftRook = gameScript.GetPosition(0, 0);
            GameObject rightRook = gameScript.GetPosition(7, 0);
            if(leftRook != null) {
                if (CheckCastling(this, leftRook.GetComponent<Chessman>())) {
                    MovePlateCastlingSpawn(xBoard - 2, yBoard, leftRook);
                }
            }
            if(rightRook != null) {
                if (CheckCastling(this, rightRook.GetComponent<Chessman>())) {
                    MovePlateCastlingSpawn(xBoard + 2, yBoard, rightRook);
                }
            }
        } else {
            GameObject leftRook = gameScript.GetPosition(0, 7);
            GameObject rightRook = gameScript.GetPosition(7, 7);
            if(leftRook != null) {
                if (CheckCastling(this, leftRook.GetComponent<Chessman>())) {
                    MovePlateCastlingSpawn(xBoard - 2, yBoard, leftRook);
                }
            }
            if(rightRook != null) {
                if (CheckCastling(this, rightRook.GetComponent<Chessman>())) {
                    MovePlateCastlingSpawn(xBoard + 2, yBoard, rightRook);
                }
            }
        }
    }

    public void PointMovePlate(int x, int y) {
        Game gameScript = controller.GetComponent<Game>();
        // Debug.Log("Position: x: " + x + " y: " + y);
        if (gameScript.PositionOnBoard(x, y)) {
            GameObject chessPiece = gameScript.GetPosition(x, y);

            if (chessPiece == null) {
                MovePlateSpawn(x, y);
            } else if (chessPiece.GetComponent<Chessman>().player != player) {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void WhitePawnMovePlate(int x, int y) {
        Game gameScript = controller.GetComponent<Game>();
        GameObject refToPossibleEnPassantPawn = gameScript.GetRefToPossibleEnPassantPawn();

        if (gameScript.PositionOnBoard(x, y)) {
            if (gameScript.GetPosition(x, y) == null) {
                MovePlateSpawn(x, y);
                if (!hasMoved) {
                    MovePlateSpawn(x, y + 1);
                }
            }

            if (refToPossibleEnPassantPawn) {
                if (gameScript.GetPosition(x + 1, y - 1) == refToPossibleEnPassantPawn) {
                    MovePlateAttackSpawn(x + 1, y);
                }

                if (gameScript.GetPosition(x - 1, y - 1) == refToPossibleEnPassantPawn) {
                    MovePlateAttackSpawn(x - 1, y);
                }
            }

            if (gameScript.PositionOnBoard(x + 1, y) &&
                gameScript.GetPosition(x + 1, y) != null &&
                gameScript.GetPosition(x + 1, y).GetComponent<Chessman>().player != player
            ) {
                MovePlateAttackSpawn(x + 1, y);
            }

            if (gameScript.PositionOnBoard(x - 1, y) &&
                gameScript.GetPosition(x - 1, y) != null &&
                gameScript.GetPosition(x - 1, y).GetComponent<Chessman>().player != player
            ) {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
    }

    public void BlackPawnMovePlate(int x, int y) {
        Game gameScript = controller.GetComponent<Game>();
        GameObject refToPossibleEnPassantPawn = gameScript.GetRefToPossibleEnPassantPawn();

        if (gameScript.PositionOnBoard(x, y)) {
            if (gameScript.GetPosition(x, y) == null) {
                MovePlateSpawn(x, y);
                if (!hasMoved) {
                    MovePlateSpawn(x, y - 1);
                }
            }

            if (refToPossibleEnPassantPawn) {
                if (gameScript.GetPosition(x + 1, y + 1) == refToPossibleEnPassantPawn) {
                    MovePlateAttackSpawn(x + 1, y);
                }

                if (gameScript.GetPosition(x - 1, y + 1) == refToPossibleEnPassantPawn) {
                    MovePlateAttackSpawn(x - 1, y);
                }
            }

            if (gameScript.PositionOnBoard(x + 1, y) &&
                gameScript.GetPosition(x + 1, y) != null &&
                gameScript.GetPosition(x + 1, y).GetComponent<Chessman>().player != player
            ) {
                MovePlateAttackSpawn(x + 1, y);
            }

            if (gameScript.PositionOnBoard(x - 1, y) &&
                gameScript.GetPosition(x - 1, y) != null &&
                gameScript.GetPosition(x - 1, y).GetComponent<Chessman>().player != player
            ) {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY) {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        GameObject MovePlate = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate movePlateScript = MovePlate.GetComponent<MovePlate>();
        movePlateScript.SetReference(gameObject);
        movePlateScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY) {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        GameObject movePlate = Instantiate(this.movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate movePlateScript = movePlate.GetComponent<MovePlate>();
        movePlateScript.SetAttack(true);
        movePlateScript.SetReference(gameObject);
        movePlateScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateCastlingSpawn(int matrixX, int matrixY, GameObject rook) {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        GameObject MovePlate = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate movePlateScript = MovePlate.GetComponent<MovePlate>();
        movePlateScript.SetCastling(rook);
        movePlateScript.SetReference(gameObject);
        movePlateScript.SetCoords(matrixX, matrixY);
    }

    public bool CheckCastling(Chessman king, Chessman rook) {
        Game gameScript = controller.GetComponent<Game>();

        if(king.hasMoved == true || rook.hasMoved == true) {
            return false;
        }

        if(king.xBoard > rook.xBoard) {
            for(int i = rook.xBoard + 1; i < king.xBoard; i++) {
                if(gameScript.GetPosition(i, king.yBoard)) {
                    return false;
                }
            }
        } else {
            for(int i = king.xBoard + 1; i < rook.xBoard; i++) {
                if(gameScript.GetPosition(i, king.yBoard)) {
                    return false;
                }
            }
        }

        return true;
    }
} 
