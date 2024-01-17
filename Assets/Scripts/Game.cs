using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : NetworkBehaviour
{
    //Positions and team for each chesspiece
    public GameObject chessPiece;
    public GameObject card;
    public Button resignButton;

    [SerializeField] private GameObject canvasManager;

    private NetworkVariable<GameObject>[,] positions = new NetworkVariable<GameObject>[8, 8];

    //private GameObject[,] positions = new GameObject[8, 8];

    
    //private GameObject[] playerBlack = new GameObject[16];
    private NetworkVariable<GameObject>[] playerWhitePieces = new NetworkVariable<GameObject>[16];
    private NetworkVariable<GameObject>[] playerBlackPieces = new NetworkVariable<GameObject>[16];

    [SerializeField] private GameObject playerWhiteCards;
    [SerializeField] private GameObject playerBlackCards;

    //private string currentPlayer = "white";

    //true  - white's turn
    //false - black's turn
    private NetworkVariable<FixedString32Bytes> currentPlayer = new NetworkVariable<FixedString32Bytes>("white");

    private NetworkVariable<bool> gameOver = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> gameHasStarted = new NetworkVariable<bool>(false);

    private NetworkVariable<bool> piecesHaveSpawned = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isServerReady = new NetworkVariable<bool>(false);

    private  GameObject refToPossibleEnPassantPawn = null;
    private Player[] players = new Player[2];
    private NetworkVariable<FixedString32Bytes> playerName1 = new NetworkVariable<FixedString32Bytes>("player1");
    private NetworkVariable<FixedString32Bytes> playerName2 = new NetworkVariable<FixedString32Bytes>("player2");
    private NetworkVariable<FixedString32Bytes> playerRating1 = new NetworkVariable<FixedString32Bytes>("rating1");
    private NetworkVariable<FixedString32Bytes> playerRating2 = new NetworkVariable<FixedString32Bytes>("rating2");
    private NetworkVariable<FixedString32Bytes> playerId1 = new NetworkVariable<FixedString32Bytes>("id1");
    private NetworkVariable<FixedString32Bytes> playerId2 = new NetworkVariable<FixedString32Bytes>("id2");

    [ServerRpc (RequireOwnership = false)]
    public void InitializePiecesServerRpc() {
        GameObject lobbyInfo = GameObject.FindGameObjectWithTag("Lobby");
        players = lobbyInfo.GetComponent<ChessLobby>().GetPlayers();

        
        

        Debug.Log("ASTEA SUNT VALORILE DIN PLAYERS:");
        Debug.Log(players[0].Data["PlayerName"].Value);
        Debug.Log(players[0].Data["PlayerRating"].Value);
        Debug.Log(players[1].Data["PlayerName"].Value);
        Debug.Log(players[1].Data["PlayerRating"].Value);

        SetPlayersServerRpc(
            players[0].Data["PlayerName"].Value,
            players[0].Data["PlayerRating"].Value,
            players[1].Data["PlayerName"].Value,
            players[1].Data["PlayerRating"].Value
        );

        playerName1.Value = players[0].Data["PlayerName"].Value;
        playerName2.Value = players[1].Data["PlayerName"].Value;
        playerId1.Value = players[0].Data["PlayerId"].Value;
        playerId2.Value = players[0].Data["PlayerId"].Value;
        playerRating1.Value = players[0].Data["PlayerRating"].Value;
        playerRating2.Value = players[1].Data["PlayerRating"].Value;

        playerWhitePieces = new NetworkVariable<GameObject>[] {
            CreatePiece("white_rook", 0, 0),
            CreatePiece("white_knight", 1, 0),
            CreatePiece("white_bishop", 2, 0),
            CreatePiece("white_queen", 3, 0),
            CreatePiece("white_king", 4, 0),
            CreatePiece("white_bishop", 5, 0),
            CreatePiece("white_knight", 6, 0),
            CreatePiece("white_rook", 7, 0),
            CreatePiece("white_pawn", 0, 1),
            CreatePiece("white_pawn", 1, 1),
            CreatePiece("white_pawn", 2, 1),
            CreatePiece("white_pawn", 3, 1),
            CreatePiece("white_pawn", 4, 1),
            CreatePiece("white_pawn", 5, 1),
            CreatePiece("white_pawn", 6, 1),
            CreatePiece("white_pawn", 7, 1)
        };

        playerBlackPieces = new NetworkVariable<GameObject>[] {
            CreatePiece("black_rook", 0, 7),
            CreatePiece("black_knight", 1, 7),
            CreatePiece("black_bishop", 2, 7),
            CreatePiece("black_queen", 3, 7),
            CreatePiece("black_king", 4, 7),
            CreatePiece("black_bishop", 5, 7),
            CreatePiece("black_knight", 6, 7),
            CreatePiece("black_rook", 7, 7),
            CreatePiece("black_pawn", 0, 6),
            CreatePiece("black_pawn", 1, 6),
            CreatePiece("black_pawn", 2, 6),
            CreatePiece("black_pawn", 3, 6),
            CreatePiece("black_pawn", 4, 6),
            CreatePiece("black_pawn", 5, 6),
            CreatePiece("black_pawn", 6, 6),
            CreatePiece("black_pawn", 7, 6)
        };

        

        // Set all piece positions on the board
        for (int i = 0; i < playerWhitePieces.Length; i++){
            Debug.Log(playerBlackPieces[i].Value);
            SetPositionServerRpc(playerBlackPieces[i].Value.GetComponent<NetworkObject>());
            SetPositionServerRpc(playerWhitePieces[i].Value.GetComponent<NetworkObject>());
        }
        
        InitializationDoneServerRpc(
            players[0].Data["PlayerName"].Value,
            players[0].Data["PlayerRating"].Value,
            players[1].Data["PlayerName"].Value,
            players[1].Data["PlayerRating"].Value
        );

        CreateCardsForPlayerClientRpc();
    }

    [ClientRpc]
    public void CreateCardsForPlayerClientRpc() {
        CardGenerator cardGenerator = this.GetComponent<CardGenerator>();

        if (IsHost) {
            cardGenerator.CreateCard("white_pawn", playerWhiteCards);
            cardGenerator.CreateCard("white_pawn", playerWhiteCards);
            cardGenerator.CreateCard("white_pawn", playerWhiteCards);
            cardGenerator.CreateCard("white_rook", playerWhiteCards);
            cardGenerator.CreateCard("white_knight", playerWhiteCards);
            cardGenerator.CreateCard("white_bishop", playerWhiteCards);
            cardGenerator.CreateCard("white_queen", playerWhiteCards);
        } else {
            cardGenerator.CreateCard("black_pawn", playerWhiteCards);
            cardGenerator.CreateCard("black_pawn", playerWhiteCards);
            cardGenerator.CreateCard("black_pawn", playerWhiteCards);
            cardGenerator.CreateCard("black_rook", playerWhiteCards);
            cardGenerator.CreateCard("black_knight", playerWhiteCards);
            cardGenerator.CreateCard("black_bishop", playerWhiteCards);
            cardGenerator.CreateCard("black_queen", playerWhiteCards);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void CreatePieceServerRpc(string name, int x, int y) {
        NetworkVariable<GameObject> networkVariable = CreatePiece(name, x, y);
        SetPositionServerRpc(networkVariable.Value.GetComponent<NetworkObject>());
    }

    public NetworkVariable<GameObject> CreatePiece(string name, int x, int y) {
        // Debug.Log("Creating " + name);
        GameObject obj = Instantiate(chessPiece, new Vector3(0, 0, 95f), Quaternion.identity);
        Chessman chessmanScript = obj.GetComponent<Chessman>();
        
        // Debug.Log(obj.GetComponent<SpriteRenderer>().sprite.name);
        obj.GetComponent<NetworkObject>().Spawn(true);

        Debug.Log(IsHost);
        Debug.Log(IsClient);
        Debug.Log(IsServer);

        if (IsHost) {
            do {
                Debug.Log("Checking Server...");
                CheckIfServerIsReadyServerRpc();
            } while (isServerReady.Value == false);
        }

        Debug.Log("Intram in serverRpc");
        CreatePieceServerRpc(obj.GetComponent<NetworkObject>(), name, x, y);
        Debug.Log("Iesim in serverRpc");

        // Debug.Log(obj.name);
        // Debug.Log(obj.GetComponent<Chessman>().GetXBoard());
        // Debug.Log(obj.GetComponent<Chessman>().GetYBoard());
        // Debug.Log(obj);

        return new NetworkVariable<GameObject>(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreatePieceServerRpc(NetworkObjectReference objRef, string name, int x, int y) {
        Debug.Log("ServerRpc");
        CreatePieceClientRpc(objRef, name, x, y);
    }

    [ClientRpc]
    private void CreatePieceClientRpc(NetworkObjectReference objRef, string name, int x, int y) {
        Debug.Log("ClientRpc");
        objRef.TryGet(out NetworkObject obj);
        Chessman chessmanScript = obj.GetComponent<Chessman>();

        chessmanScript.name = name;
        chessmanScript.SetXBoardServerRpc(x);
        chessmanScript.SetYBoardServerRpc(y);
        chessmanScript.SetHasMovedServerRpc(objRef ,false);
        chessmanScript.Activate();
    }

    // public GameObject CreateCard() {

    // }

    [ServerRpc(RequireOwnership = false)]
    private void CheckIfServerIsReadyServerRpc() {
        Debug.Log("Yes now it should be ready!");
        isServerReady.Value = true;
    }

    [ServerRpc (RequireOwnership = false)]
    public void SetPositionServerRpc(NetworkObjectReference obj) {
        SetPositionClientRpc(obj);
        // Chessman chessmanScript = obj.GetComponent<Chessman>();

        // positions[chessmanScript.GetXBoard(), chessmanScript.GetYBoard()] = new NetworkVariable<GameObject>(obj);
        // Debug.Log(chessmanScript.GetXBoard());
        // Debug.Log(chessmanScript.GetYBoard());
        // Debug.Log("Object added in positions: " + positions[chessmanScript.GetXBoard(), chessmanScript.GetYBoard()]);
    }

    [ClientRpc]
    public void SetPositionClientRpc(NetworkObjectReference objectReference) {
        objectReference.TryGet(out NetworkObject obj);
        Debug.Log(obj);
        Chessman chessmanScript = obj.GetComponent<Chessman>();

        positions[chessmanScript.GetXBoard(), chessmanScript.GetYBoard()] = new NetworkVariable<GameObject>(obj.GameObject());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPositionEmptyServerRpc(int x, int y) {
        SetPositionEmptyClientRpc(x, y);
    }

    [ClientRpc]
    public void SetPositionEmptyClientRpc(int x, int y) {
        positions[x, y] = null;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetRefToPossibleEnPassantPawnServerRpc(NetworkObjectReference refToPossibleEnPassantPawn, bool setItNull) {
        SetRefToPossibleEnPassantPawnClientRpc(refToPossibleEnPassantPawn, setItNull);
    }

    [ClientRpc]
    public void SetRefToPossibleEnPassantPawnClientRpc(NetworkObjectReference refToPossibleEnPassantPawn, bool setItNull) {

        if(setItNull) {
            this.refToPossibleEnPassantPawn = null;
            return;
        }

        refToPossibleEnPassantPawn.TryGet(out NetworkObject obj);
        this.refToPossibleEnPassantPawn = refToPossibleEnPassantPawn;
    }

    public NetworkVariable<GameObject>[] GetPlayerWhitePieces() {
        return playerWhitePieces;
    }

    public int GetPlayerWhitePieceByNOR(NetworkObjectReference networkObjectReference) {
        for (int i = 0; i < playerWhitePieces.Length; i++)
        {
            // Assuming GameObject has an appropriate equality check (e.g., comparing an ID or some property)
            if (playerWhitePieces[i].Equals(networkObjectReference))
            {
                return i; // Return the position if found
            }
        }

        return -1;
    }

    public int GetPlayerBlackPieceByNOR(NetworkObjectReference networkObjectReference) {
        for (int i = 0; i < playerBlackPieces.Length; i++)
        {
            // Assuming GameObject has an appropriate equality check (e.g., comparing an ID or some property)
            if (playerBlackPieces[i].Equals(networkObjectReference))
            {
                return i; // Return the position if found
            }
        }

        return -1;
    }

    public NetworkVariable<GameObject>[] GetPlayerBlackPieces() {
        return playerBlackPieces;
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
        return currentPlayer.Value.ToString();
    }

    public GameObject GetRefToPossibleEnPassantPawn() {
        return refToPossibleEnPassantPawn;
    }

    public bool PositionOnBoard(int x, int y) {
        if(x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public bool IsGameOver() {
        return gameOver.Value;
    }

    public void NextTurn() {
        ChangeCurrentPlayerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeCurrentPlayerServerRpc() {
        if (currentPlayer.Value == "white") {
            currentPlayer.Value = "black";
        } else {
            currentPlayer.Value = "white";
        }
    }

    public void Update()
    {
        if (gameOver.Value == true && Input.GetMouseButtonDown(0)) {
            //SetGameOverServerRpc(false);

            SceneManager.LoadScene("MainMenuScene");
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void WinnerServerRpc(FixedString32Bytes playerWinner) {
        WinnerClientRpc(playerWinner);
    }

    [ClientRpc]
    public void WinnerClientRpc(FixedString32Bytes playerWinner) {
        if (gameOver.Value == false) {
            SetGameOverServerRpc(true);

            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
            GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

            GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;

            float f = float.Parse(playerRating1.Value.ToString());

            bool isWhiteTheWinner;

            if (playerWinner == "white") {
                isWhiteTheWinner = true;
            } else {
                isWhiteTheWinner = false;
            }

            int[] results = this.GetComponent<EloCalculator>().EloRating(
                playerId1.Value.ToString(),
                playerId2.Value.ToString(),
                playerName1.Value.ToString(),
                playerName2.Value.ToString(),
                float.Parse(playerRating1.Value.ToString()),
                float.Parse(playerRating2.Value.ToString()),
                30,
                isWhiteTheWinner
            );

            if (IsHost) {
                this.GetComponent<EloCalculator>().AddDocumentToCollection(
                    PlayerPrefs.GetString("userId"),
                    PlayerPrefs.GetString("username"),
                    results[0]
                );
                PlayerPrefs.SetString("rating", results[0].ToString());
            } else {
                this.GetComponent<EloCalculator>().AddDocumentToCollection(
                    PlayerPrefs.GetString("userId"),
                    PlayerPrefs.GetString("username"),
                    results[1]
                );
                PlayerPrefs.SetString("rating", results[1].ToString());
            }
        }
    }

    public void Resign() {

        if (IsHost) {
            WinnerServerRpc("black");
        } else {
            WinnerServerRpc("white");
        }

        
        Debug.Log("You resigned the game");
        //SceneManager.LoadScene("MainMenuScene");
    }

    [ServerRpc (RequireOwnership = false)]
    public void DestroyPieceServerRpc(NetworkObjectReference piece) {
        Debug.Log("I am going to destroy this on the server :D");
        //DestroyPieceClientRpc(piece);
        piece.TryGet(out NetworkObject obj);
        obj.GetComponent<NetworkObject>().Despawn();
        //Destroy(obj);
        //OnNetworkDespawn();
    }

    [ClientRpc]
    public void DestroyPieceClientRpc(NetworkObjectReference piece) {
        Debug.Log("I am going to destroy this on the client :D");
        piece.TryGet(out NetworkObject obj);
        Destroy(obj);
    }

    [ServerRpc (RequireOwnership = false)]
    public void SetGameOverServerRpc(bool state) {
        gameOver.Value = state;
    }

    [ServerRpc (RequireOwnership = false)]
    public void InitializationDoneServerRpc(string palyer1name, string player1rating, string player2name, string player2rating) {
        InitializationDoneClientRpc(palyer1name, player1rating, player2name, player2rating);
    }

    [ClientRpc]
    public void InitializationDoneClientRpc(string palyer1name, string player1rating, string player2name, string player2rating) {
        Debug.Log(palyer1name);
        //players[0].Data["PlayerName"].Value = playerName1.Value.ToString();
        //players[1].Data["PlayerName"].Value = playerName2.Value.ToString();
        //players[0].Data["PlayerRating"].Value = playerRating1.Value.ToString();
        //players[1].Data["PlayerRating"].Value = playerRating2.Value.ToString();

        Debug.Log("ASTEA SUNT VALORILE DIN PLAYERS:");
        //Debug.Log(players[0].Data["PlayerName"].Value);
        //Debug.Log(players[0].Data["PlayerRating"].Value);
        //Debug.Log(players[1].Data["PlayerName"].Value);
        //Debug.Log(players[1].Data["PlayerRating"].Value);

        canvasManager.GetComponent<CanvasManager>().LoadingIsDone(
            palyer1name,
            player2name,
            player1rating,
            player2rating
        );
        resignButton.onClick.AddListener(Resign);
    }

    [ServerRpc]
    public void SetPlayersServerRpc(string palyer1name, string player1rating, string player2name, string player2rating) {
        Debug.Log(palyer1name);
        playerName1.Value = palyer1name;
        playerRating1.Value = player1rating;
        playerName2.Value = player2name;
        playerRating2.Value = player2rating;
        SetPlayersClientRpc(palyer1name, player1rating, player2name, player2rating);
    }

    [ClientRpc]
    public void SetPlayersClientRpc(string palyer1name, string player1rating, string player2name, string player2rating) {
        Debug.Log(palyer1name);
    }

    public void SetGameHasStarted(bool gameHasStarted) {
        this.gameHasStarted.Value = gameHasStarted;
    }

    public bool GetGameHasStarted() {
        return gameHasStarted.Value;
    }

}
