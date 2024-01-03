using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;
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

    void Awake()
    {
        resignButton.gameObject.SetActive(false);
        opponentImage.gameObject.SetActive(false);
        opponentName.gameObject.SetActive(false);
    }

    public void setText(string text) {
        loadingText.text = text;
    }

    public void loadingIsDone(string playerName1, string playerName2, string playerRating1, string playerRating2) {
        Debug.Log("Hei distrugem si noi ceva?");
        resignButton.gameObject.SetActive(true);
        opponentImage.gameObject.SetActive(true);
        opponentName.gameObject.SetActive(true);

        loadingText.gameObject.SetActive(false);
        loadingBackground.SetActive(false);
        loadingCircle.gameObject.SetActive(false);

        opponentName.text = "Opponent Name\n1000";
        if(!IsHost) {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            camera.GetComponent<Transform>().rotation = new Quaternion(0,0, 180, 0);
            opponentName.text = playerName1 + "\n" + playerRating1;
        } else {
            GameObject opponentImage = GameObject.FindGameObjectWithTag("OpponentImage");
            opponentImage.GetComponent<Image>().sprite = blackKing;
            opponentName.text = playerName2 + "\n" + playerRating2;
        }
    }
}
