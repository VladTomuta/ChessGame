using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : NetworkBehaviour
{
    [SerializeField] private Button resignButton;
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

    void Awake()
    {
        resignButton.gameObject.SetActive(false);
        opponentImage.gameObject.SetActive(false);
        opponentName.gameObject.SetActive(false);
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
}
