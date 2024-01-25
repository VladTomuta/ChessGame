using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : NetworkBehaviour
{

    //Positions and team for each chesspiece
    public GameObject chessPiece;
    public Button resignButton;

    private NetworkVariable<GameObject>[,] positions = new NetworkVariable<GameObject>[8, 8];

    //private GameObject[,] positions = new GameObject[8, 8];

    private NetworkVariable<GameObject>[] playerBlack = new NetworkVariable<GameObject>[16];
    //private GameObject[] playerBlack = new GameObject[16];
    private NetworkVariable<GameObject>[] playerWhite = new NetworkVariable<GameObject>[16];

    private string currentPlayer = "white";
    private bool gameOver = false;


    private NetworkVariable<bool> piecesHaveSpawned = new NetworkVariable<bool>(false);

   private  GameObject refToPossibleEnPassantPawn = null;

    // Start is called before the first frame update
    // void Start()
    // {
    //     resignButton.onClick.AddListener(Resign);

    //     Debug.Log("Piece have spawned? " + piecesHaveSpawned.Value);

    //     if (!piecesHaveSpawned.Value) {
    //         playerWhite = new GameObject[] {
    //             // Create("white_rook", 0, 0),
    //             // Create("white_knight", 1, 0),
    //             // Create("white_bishop", 2, 0),
    //             // Create("white_queen", 3, 0),
    //             Create("white_king", 4, 0),
    //             // Create("white_bishop", 5, 0),
    //             // Create("white_knight", 6, 0),
    //             // Create("white_rook", 7, 0),
    //             // Create("white_pawn", 0, 1),
    //             // Create("white_pawn", 1, 1),
    //             // Create("white_pawn", 2, 1),
    //             // Create("white_pawn", 3, 1),
    //             // Create("white_pawn", 4, 1),
    //             // Create("white_pawn", 5, 1),
    //             // Create("white_pawn", 6, 1),
    //             // Create("white_pawn", 7, 1)
    //         };

    //         playerBlack = new GameObject[] {
    //             // Create("black_rook", 0, 7),
    //             // Create("black_knight", 1, 7),
    //             // Create("black_bishop", 2, 7),
    //             // Create("black_queen", 3, 7),
    //             Create("black_king", 4, 7),
    //             // Create("black_bishop", 5, 7),
    //             // Create("black_knight", 6, 7),
    //             // Create("black_rook", 7, 7),
    //             // Create("black_pawn", 0, 6),
    //             // Create("black_pawn", 1, 6),
    //             // Create("black_pawn", 2, 6),
    //             // Create("black_pawn", 3, 6),
    //             // Create("black_pawn", 4, 6),
    //             // Create("black_pawn", 5, 6),
    //             // Create("black_pawn", 6, 6),
    //             // Create("black_pawn", 7, 6)
    //         };

    //         // Set all piece positions on the board
    //         for (int i = 0; i < playerWhite.Length; i++){
    //             SetPosition(playerBlack[i]);
    //             SetPosition(playerWhite[i]);
    //         }

    //         piecesHaveSpawned.Value = true;
    //     }

        
    // }

    public void InitializePieces() {
        // Debug.Log("Piece have spawned? " + piecesHaveSpawned.Value);

        if (!piecesHaveSpawned.Value) {
            playerWhite = new NetworkVariable<GameObject>[] {
                Create("white_rook", 0, 0),
                Create("white_knight", 1, 0),
                Create("white_bishop", 2, 0),
                Create("white_queen", 3, 0),
                Create("white_king", 4, 0),
                Create("white_bishop", 5, 0),
                Create("white_knight", 6, 0),
                Create("white_rook", 7, 0),
                Create("white_pawn", 0, 1),
                Create("white_pawn", 1, 1),
                Create("white_pawn", 2, 1),
                Create("white_pawn", 3, 1),
                Create("white_pawn", 4, 1),
                Create("white_pawn", 5, 1),
                Create("white_pawn", 6, 1),
                Create("white_pawn", 7, 1)
            };

            playerBlack = new NetworkVariable<GameObject>[] {
                Create("black_rook", 0, 7),
                Create("black_knight", 1, 7),
                Create("black_bishop", 2, 7),
                Create("black_queen", 3, 7),
                Create("black_king", 4, 7),
                Create("black_bishop", 5, 7),
                Create("black_knight", 6, 7),
                Create("black_rook", 7, 7),
                Create("black_pawn", 0, 6),
                Create("black_pawn", 1, 6),
                Create("black_pawn", 2, 6),
                Create("black_pawn", 3, 6),
                Create("black_pawn", 4, 6),
                Create("black_pawn", 5, 6),
                Create("black_pawn", 6, 6),
                Create("black_pawn", 7, 6)
            };

            // Set all piece positions on the board
            for (int i = 0; i < playerWhite.Length; i++){
                SetPosition(playerBlack[i].Value);
                SetPosition(playerWhite[i].Value);
            }

            //piecesHaveSpawned.Value = true;
        }
    }

    public NetworkVariable<GameObject> Create(string name, int x, int y) {
        // Debug.Log("Creating " + name);
        GameObject obj = Instantiate(chessPiece, new Vector3(0, 0, 95f), Quaternion.identity);
        Chessman chessmanScript = obj.GetComponent<Chessman>();
        
        // Debug.Log(obj.GetComponent<SpriteRenderer>().sprite.name);
        obj.GetComponent<NetworkObject>().Spawn(true);

        // Debug.Log(obj.name);
        // Debug.Log(obj.GetComponent<Chessman>().GetXBoard());
        // Debug.Log(obj.GetComponent<Chessman>().GetYBoard());
        // Debug.Log(obj);

        return new NetworkVariable<GameObject>(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateServerRcp(GameObject obj, string name, int x, int y) {
        CreateClientRcp(obj, name, x, y);
    }

    [ClientRpc]
    private void CreateClientRcp(GameObject obj, string name, int x, int y) {
        Chessman chessmanScript = obj.GetComponent<Chessman>();

        chessmanScript.name = name;
        chessmanScript.SetXBoard(x);
        chessmanScript.SetYBoard(y);
        chessmanScript.SetHasMoved(false);
        chessmanScript.Activate();
    }

    public void SetPosition(GameObject obj) {
        Chessman chessmanScript = obj.GetComponent<Chessman>();

        positions[chessmanScript.GetXBoard(), chessmanScript.GetYBoard()] = new NetworkVariable<GameObject>(obj);
        // Debug.Log(chessmanScript.GetXBoard());
        // Debug.Log(chessmanScript.GetYBoard());
        // Debug.Log("Object added in positions: " + positions[chessmanScript.GetXBoard(), chessmanScript.GetYBoard()]);
    }

    public void SetPositionEmpty(int x, int y) {
        positions[x, y] = null;
    }

    public void SetRefToPossibleEnPassantPawn(GameObject refToPossibleEnPassantPawn) {
        this.refToPossibleEnPassantPawn = refToPossibleEnPassantPawn;
    }

    public GameObject GetPosition(int x, int y) {
        // for (int i = 0; i < 8; i++) {
        //     for (int j = 0; j < 8; j++) {
        //         Debug.Log("i = " + i + "\nj = " + j + "\nValue: " + positions[i, j].Value);
        //     }
        // }

        if(positions[x, y] == null) {
            return null;
        }
        return positions[x, y].Value;
    }

    public string GetCurrentPlayer() {
        return currentPlayer;
    }

    public GameObject GetRefToPossibleEnPassantPawn() {
        return refToPossibleEnPassantPawn;
    }

    public bool PositionOnBoard(int x, int y) {
        if(x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public bool IsGameOver() {
        return gameOver;
    }

    public void NextTurn() {
        if (currentPlayer == "white") {
            currentPlayer = "black";
        } else {
            currentPlayer = "white";
        }
    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0)) {
            gameOver = false;

            SceneManager.LoadScene("MainMenuScene");
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            InitializePieces();
        }
    }

    public void Winner(string playerWinner) {
        gameOver = true;

        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }

    public void Resign() {
        Debug.Log("You resigned the game");
        SceneManager.LoadScene("MainMenuScene");
    }
}
