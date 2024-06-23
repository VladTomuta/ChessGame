using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Game : NetworkBehaviour
{
    public GameObject chessPiece;
    public Button resignButton;
    public Button drawButton;

    [SerializeField] private GameObject canvasManager;

    private NetworkVariable<GameObject>[,] positions = new NetworkVariable<GameObject>[8, 8];

    private NetworkVariable<GameObject>[] playerWhitePieces = new NetworkVariable<GameObject>[16];
    private NetworkVariable<GameObject>[] playerBlackPieces = new NetworkVariable<GameObject>[16];

    [SerializeField] private GameObject playerWhiteCards;
    [SerializeField] private GameObject playerBlackCards;

    //true  - white's turn
    //false - black's turn
    private NetworkVariable<FixedString32Bytes> currentPlayer = new NetworkVariable<FixedString32Bytes>("white");

    private NetworkVariable<bool> gameOver = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> gameHasStarted = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isServerReady = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isDrawOffered = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> initializationHasStarted = new NetworkVariable<bool>(false);
    private bool didIOfferDraw = false;
    private GameObject refToPossibleEnPassantPawn = null;
    private Player[] players = new Player[2];
    private NetworkVariable<FixedString32Bytes> playerName1 = new NetworkVariable<FixedString32Bytes>("player1");
    private NetworkVariable<FixedString32Bytes> playerName2 = new NetworkVariable<FixedString32Bytes>("player2");
    private NetworkVariable<FixedString32Bytes> playerRating1 = new NetworkVariable<FixedString32Bytes>("rating1");
    private NetworkVariable<FixedString32Bytes> playerRating2 = new NetworkVariable<FixedString32Bytes>("rating2");
    private NetworkVariable<FixedString32Bytes> playerId1 = new NetworkVariable<FixedString32Bytes>("id1");
    private NetworkVariable<FixedString32Bytes> playerId2 = new NetworkVariable<FixedString32Bytes>("id2");

    [ServerRpc (RequireOwnership = false)]
    public void InitializePiecesServerRpc(ServerRpcParams serverRpcParams = default) {

        Debug.Log("Initialize done by " + serverRpcParams.Receive.SenderClientId);

        if (serverRpcParams.Receive.SenderClientId != 1) 
            return;

        if (initializationHasStarted.Value)
            return;

        initializationHasStarted.Value = true;

        GameObject lobbyInfo = GameObject.FindGameObjectWithTag("Lobby");
        players = lobbyInfo.GetComponent<ChessLobby>().GetPlayers();

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
            CreatePiece("white_king", 4, 0),
            CreatePiece("white_pawn", 3, 1),
            CreatePiece("white_pawn", 4, 1),
            CreatePiece("white_pawn", 5, 1)
        };

        playerBlackPieces = new NetworkVariable<GameObject>[] {
            CreatePiece("black_king", 4, 7),
            CreatePiece("black_pawn", 3, 6),
            CreatePiece("black_pawn", 4, 6),
            CreatePiece("black_pawn", 5, 6)
        };

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

        CreateCardsForPlayerServerRpc();
    }

    [ServerRpc]
    public void CreateCardsForPlayerServerRpc() {
        CardGenerator cardGenerator = this.GetComponent<CardGenerator>();

        cardGenerator.CreateCard("white_pawn", playerWhiteCards, "white");
        cardGenerator.CreateCard("white_pawn", playerWhiteCards, "white");
        cardGenerator.CreateCard("white_pawn", playerWhiteCards, "white");
        cardGenerator.CreateCard("white_rook", playerWhiteCards, "white");
        cardGenerator.CreateCard("white_knight", playerWhiteCards, "white");
        cardGenerator.CreateCard("white_bishop", playerWhiteCards, "white");
        cardGenerator.CreateCard("white_queen", playerWhiteCards, "white");
        cardGenerator.CreateCard("black_pawn", playerBlackCards, "black");
        cardGenerator.CreateCard("black_pawn", playerBlackCards, "black");
        cardGenerator.CreateCard("black_pawn", playerBlackCards, "black");
        cardGenerator.CreateCard("black_rook", playerBlackCards, "black");
        cardGenerator.CreateCard("black_knight", playerBlackCards, "black");
        cardGenerator.CreateCard("black_bishop", playerBlackCards, "black");
        cardGenerator.CreateCard("black_queen", playerBlackCards, "black");
    }

    [ServerRpc (RequireOwnership = false)]
    public void CreatePieceServerRpc(string name, int x, int y) {
        NetworkVariable<GameObject> networkVariable = CreatePiece(name, x, y);
        SetPositionServerRpc(networkVariable.Value.GetComponent<NetworkObject>());
    }

    public NetworkVariable<GameObject> CreatePiece(string name, int x, int y) {
        GameObject obj = Instantiate(chessPiece, new Vector3(0, 0, 95f), Quaternion.identity);
        Chessman chessmanScript = obj.GetComponent<Chessman>();
    
        obj.GetComponent<NetworkObject>().Spawn(true);

        if (IsHost) {
            do {
                Debug.Log("Checking Server...");
                CheckIfServerIsReadyServerRpc();
            } while (isServerReady.Value == false);
        }

        CreatePieceServerRpc(obj.GetComponent<NetworkObject>(), name, x, y);

        return new NetworkVariable<GameObject>(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreatePieceServerRpc(NetworkObjectReference objRef, string name, int x, int y) {
        CreatePieceClientRpc(objRef, name, x, y);
    }

    [ClientRpc]
    private void CreatePieceClientRpc(NetworkObjectReference objRef, string name, int x, int y) {
        objRef.TryGet(out NetworkObject obj);
        Chessman chessmanScript = obj.GetComponent<Chessman>();

        chessmanScript.name = name;
        chessmanScript.SetXBoardServerRpc(x);
        chessmanScript.SetYBoardServerRpc(y);
        chessmanScript.SetHasMovedServerRpc(objRef ,false);
        chessmanScript.Activate();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckIfServerIsReadyServerRpc() {
        isServerReady.Value = true;
    }

    [ServerRpc (RequireOwnership = false)]
    public void SetPositionServerRpc(NetworkObjectReference obj) {
        SetPositionClientRpc(obj);
    }

    [ClientRpc]
    public void SetPositionClientRpc(NetworkObjectReference objectReference) {
        objectReference.TryGet(out NetworkObject obj);
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
            if (playerWhitePieces[i].Equals(networkObjectReference))
            {
                return i;
            }
        }

        return -1;
    }

    public int GetPlayerBlackPieceByNOR(NetworkObjectReference networkObjectReference) {
        for (int i = 0; i < playerBlackPieces.Length; i++)
        {
            if (playerBlackPieces[i].Equals(networkObjectReference))
            {
                return i;
            }
        }

        return -1;
    }

    public NetworkVariable<GameObject>[] GetPlayerBlackPieces() {
        return playerBlackPieces;
    }

    public GameObject GetPosition(int x, int y) {
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

        Debug.Log("currentPlayer.Value = " + currentPlayer.Value);

        ChangeCurrentPlayerServerRpc();

        if(didIOfferDraw)
            return;

        SetDrawOfferTextActive(false);
        SetDidIOfferDrawServerRpc(false);
        SetIsDrawOfferedServerRpc(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeCurrentPlayerServerRpc() {

        if (currentPlayer.Value == "white") {
            currentPlayer.Value = "black";
        } else {
            currentPlayer.Value = "white";
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void WinnerServerRpc(FixedString32Bytes playerWinner, FixedString32Bytes winCondition) {
        SetDrawOfferTextActive(false);
        SetDidIOfferDrawServerRpc(false);
        SetIsDrawOfferedServerRpc(false);
        WinnerClientRpc(playerWinner, winCondition);
    }

    [ClientRpc]
    public void WinnerClientRpc(FixedString32Bytes playerWinner, FixedString32Bytes winCondition) {
        if (gameOver.Value == false) {
            SetGameOverServerRpc(true);

            float f = float.Parse(playerRating1.Value.ToString());

            int gameResult = 0; // 0 - tie, 1 - white won, 2 - black won

            switch (playerWinner.ToString()) {
                case "white": 
                    gameResult = 1;
                    break;
                case "black": 
                    gameResult = 2;
                    break;
                case "tie": 
                    gameResult = 0;
                    break;
            }

            int[] results = this.GetComponent<EloCalculator>().EloRating(
                playerId1.Value.ToString(),
                playerId2.Value.ToString(),
                playerName1.Value.ToString(),
                playerName2.Value.ToString(),
                float.Parse(playerRating1.Value.ToString()),
                float.Parse(playerRating2.Value.ToString()),
                30,
                gameResult
            );

            int eloDifference;

            if (IsHost) {
                eloDifference = results[0] - int.Parse(playerRating1.Value.ToString());
            } else {
                eloDifference = results[1] - int.Parse(playerRating2.Value.ToString());
            }

            canvasManager.GetComponent<CanvasManager>().PopUpEndGameSummary(playerWinner.ToString(), winCondition.ToString(), eloDifference);

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
            WinnerServerRpc("black", "resignation");
        } else {
            WinnerServerRpc("white", "resignation");
        }
    }

    public void Draw() {

        if (didIOfferDraw)
            return;

        didIOfferDraw = true;

        Debug.Log(isDrawOffered.Value);

        if (isDrawOffered.Value) {
            WinnerServerRpc("tie", "drawAccepted");
        } else {
            SetIsDrawOfferedServerRpc(true);
            SetDrawOfferTextActive(true);
        }
    }

    public void SetDrawOfferTextActive(bool value) {
        canvasManager.GetComponent<CanvasManager>().SetDrawOfferTextActiveServerRpc(value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetIsDrawOfferedServerRpc(bool isDrawOffered) {
        this.isDrawOffered.Value = isDrawOffered;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDidIOfferDrawServerRpc(bool didIOfferDraw) {
        SetDidIOfferDrawClientRpc(didIOfferDraw);
    }

    [ClientRpc]
    public void SetDidIOfferDrawClientRpc(bool didIOfferDraw) {
        this.didIOfferDraw = didIOfferDraw;
    }

    public bool GetDidIOfferDraw() {
        return didIOfferDraw;
    }

    [ServerRpc (RequireOwnership = false)]
    public void DestroyPieceServerRpc(NetworkObjectReference piece) {
        piece.TryGet(out NetworkObject obj);
        obj.GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc]
    public void DestroyPieceClientRpc(NetworkObjectReference piece) {
        piece.TryGet(out NetworkObject obj);
        Destroy(obj);
    }

    [ServerRpc (RequireOwnership = false)]
    public void DestroyCardServerRpc(NetworkObjectReference card) {
        card.TryGet(out NetworkObject obj);
        obj.GetComponent<NetworkObject>().Despawn();
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
        canvasManager.GetComponent<CanvasManager>().LoadingIsDone(
            palyer1name,
            player2name,
            player1rating,
            player2rating
        );
        resignButton.onClick.AddListener(Resign);
        drawButton.onClick.AddListener(Draw);
    }

    [ServerRpc]
    public void SetPlayersServerRpc(string palyer1name, string player1rating, string player2name, string player2rating) {
        playerName1.Value = palyer1name;
        playerRating1.Value = player1rating;
        playerName2.Value = player2name;
        playerRating2.Value = player2rating;
    }


    public void SetGameHasStarted(bool gameHasStarted) {
        this.gameHasStarted.Value = gameHasStarted;
    }

    public bool GetGameHasStarted() {
        return gameHasStarted.Value;
    }
}
