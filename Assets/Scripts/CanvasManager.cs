using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : NetworkBehaviour
{
    [SerializeField] private Button resignButton;
    [SerializeField] private Button drawButton;
    [SerializeField] private TMP_Text drawOfferText;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private GameObject loadingBackground;
    [SerializeField] private GameObject loadingCircle;
    [SerializeField] private Image opponentImage;
    [SerializeField] private Sprite blackKing;
    [SerializeField] private TMP_Text opponentName;
    [SerializeField] private Button cancelSearchButton;
    [SerializeField] private TMP_Text yourTime;
    [SerializeField] private TMP_Text opponentTime;
    [SerializeField] private GameObject whiteCards;
    [SerializeField] private GameObject blackCards;
    [SerializeField] private GameObject endGameSummary;

    void Awake()
    {
        resignButton.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(false);
        opponentImage.gameObject.SetActive(false);
        opponentName.gameObject.SetActive(false);
        drawOfferText.gameObject.SetActive(false);
    }

    void Start()
    {
        cancelSearchButton.onClick.AddListener(CancelSearch);
    }

    public void SetText(string text) {
        loadingText.text = text;
        cancelSearchButton.gameObject.SetActive(false);
    }

    public void LoadingIsDone(string playerName1, string playerName2, string playerRating1, string playerRating2) {
        resignButton.gameObject.SetActive(true);
        drawButton.gameObject.SetActive(true);
        opponentImage.gameObject.SetActive(true);
        opponentName.gameObject.SetActive(true);
        yourTime.gameObject.SetActive(true);
        opponentTime.gameObject.SetActive(true);

        loadingText.gameObject.SetActive(false);
        loadingBackground.SetActive(false);
        loadingCircle.gameObject.SetActive(false);

        opponentName.text = "Opponent Name\n1000";
        if(!IsHost) {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<Transform>().rotation = new Quaternion(0, 0, 180, 0);
            opponentName.text = playerName1 + "\n" + playerRating1;
            whiteCards.GetComponent<Transform>().rotation = new Quaternion(0, 0, 180, 0);
        } else {
            GameObject opponentImage = GameObject.FindGameObjectWithTag("OpponentImage");
            opponentImage.GetComponent<Image>().sprite = blackKing;
            opponentName.text = playerName2 + "\n" + playerRating2;
            blackCards.GetComponent<Transform>().rotation = new Quaternion(0, 0, 180, 0);
        }
    }

    public void CancelSearch() {
        GameObject chessLobby = GameObject.FindGameObjectWithTag("Lobby");
        chessLobby.GetComponent<ChessLobby>().LeaveLobby();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SetTime(string player, string time) {
        if (player == "black") {
            if(IsHost) {
                yourTime.text = time;
            } else {
                opponentTime.text = time;
            }
        } else {
            if(IsHost) {
                opponentTime.text = time;
            } else {
                yourTime.text = time;
            }
        }
    }

    public void PopUpEndGameSummary(string playerWinner, string winCondition, int eloDifference) {
        endGameSummary.SetActive(true);

        SetTextsInEndGameSummary(playerWinner, winCondition, eloDifference);

        ActivateEndGameSummaryButtons();
    }

    public void ActivateEndGameSummaryButtons() {
        Button[] buttons = endGameSummary.GetComponentsInChildren<Button>();

        foreach (Button button in buttons) {
            switch (button.name) {
                case "ReturnToMainMenuButton" :
                    button.onClick.AddListener(ReturnToMainMenu);
                    break;
                case "CloseMenuButton" :
                    button.onClick.AddListener(CloseEndGameSummary);
                    break;
            }
        }
    }

    public void CloseEndGameSummary() {
        endGameSummary.SetActive(false);
    }

    public void ReturnToMainMenu() {
        GameObject chessLobby = GameObject.FindGameObjectWithTag("Lobby");
        chessLobby.GetComponent<ChessLobby>().LeaveLobby();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SetTextsInEndGameSummary(string playerWinner, string winCondition, int eloDifference) {
        TMP_Text[] endGameTexts = endGameSummary.GetComponentsInChildren<TMP_Text>();

        foreach (TMP_Text endGameText in endGameTexts) {
            switch (endGameText.name) {
                case "ResultText" :

                    if (playerWinner == "tie") {
                        switch (winCondition) {
                            case "drawAccepted":
                                endGameText.text = "You have decided to draw the game with your opponent";
                                break;
                            case "onlyKingsLeft":
                                endGameText.text = "The game ends in a draw because the only pieces left are the kings";
                                break;
                        }
                    } else if (IsHost) {
                        if (playerWinner == "white") {
                            switch (winCondition) {
                                case "capture" :
                                    endGameText.text = "You have won the game because you captured the opponent king";
                                    break;
                                case "resignation" :
                                    endGameText.text = "You have won the game because the opponent resigned";
                                    break;
                                case "timeRanOut" :
                                    endGameText.text = "You have won the game because your opponent ran out of time";
                                    break;
                            }
                        } else {
                            switch (winCondition) {
                                case "capture" :
                                    endGameText.text = "You have lost the game because your king has been captured";
                                    break;
                                case "resignation" :
                                    endGameText.text = "You have lost the game because you have resigned";
                                    break;
                                case "timeRanOut" :
                                    endGameText.text = "You have lost the game because you ran out of time";
                                    break;
                            }
                        }
                    } else {
                        if (playerWinner == "white") {
                            switch (winCondition) {
                                case "capture" :
                                    endGameText.text = "You have lost the game because your king has been captured";
                                    break;
                                case "resignation" :
                                    endGameText.text = "You have lost the game because you have resigned";
                                    break;
                                case "timeRanOut" :
                                    endGameText.text = "You have lost the game because you ran out of time";
                                    break;
                            }
                        } else {
                            switch (winCondition) {
                                case "capture" :
                                    endGameText.text = "You have won the game because you captured the opponent king";
                                    break;
                                case "resignation" :
                                    endGameText.text = "You have won the game because the opponent resigned";
                                    break;
                                case "timeRanOut" :
                                    endGameText.text = "You have won the game because your opponent ran out of time";
                                    break;
                            }
                        }
                    }

                    break;
                case "EloGainedText" :
                    if (eloDifference >= 0) {
                        endGameText.text = "You have gained\n" + eloDifference + " elo";
                    } else {
                        endGameText.text = "You have lost\n" + (eloDifference * -1) + " elo";
                    }
                    
                    break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDrawOfferTextActiveServerRpc(bool activity) {
        SetDrawOfferTextActiveClientRpc(activity);
    }

    [ClientRpc]
    public void SetDrawOfferTextActiveClientRpc(bool activity) {
        drawOfferText.gameObject.SetActive(activity);
        SetDrawOfferTextText(GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>().GetDidIOfferDraw(), activity);
    }

    public void SetDrawOfferTextText(bool didIOfferDraw, bool activity) {
        if (didIOfferDraw) {
            drawOfferText.text = "You have offered a draw";
        } else {
            drawOfferText.text = "Your opponent has offered a draw";
            SetDrawButtonText(activity);
        }
    }

    public void SetDrawButtonText(bool activity) {
        if (activity) {
            drawButton.GetComponentInChildren<TMP_Text>().text = "Accept Draw";
        } else {
            drawButton.GetComponentInChildren<TMP_Text>().text = "Offer Draw";
        }
        
    }
}
