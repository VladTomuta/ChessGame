using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    [SerializeField] private CanvasManager canvasManager;
    private GameObject controller;
    private NetworkVariable<float> whiteTime = new NetworkVariable<float>(600f);
    private NetworkVariable<float> blackTime = new NetworkVariable<float>(600f);
    private NetworkVariable<FixedString32Bytes> newTime = new NetworkVariable<FixedString32Bytes>("10:00");
    private string currentPlayer;

    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        // if (!IsHost) {
        //     Debug.Log("Nu sunt Host? :(");
        //     return;
        // }
            

        //Debug.Log("Sunt host :D");
        
        string gameMode = PlayerPrefs.GetString("gameMode");
        string[] words = gameMode.Split();

        blackTime.Value = float.Parse(words[0]) * 60;
        whiteTime.Value = float.Parse(words[0]) * 60;
        newTime.Value = words[0] + ":00";

        if (words[0].Length == 1) {
            newTime.Value = "0" + newTime.Value;
        }

        canvasManager.SetTime("white", newTime.Value.ToString());
        canvasManager.SetTime("black", newTime.Value.ToString());

        //blackTime.Value = 600f;
        //whiteTime.Value = 600f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!controller.GetComponent<Game>().GetGameHasStarted())
            return;

        currentPlayer = controller.GetComponent<Game>().GetCurrentPlayer();

        if(currentPlayer == "white") {
            if(IsHost) {
                calculateWhiteTimeServerRpc(false);
            }
            canvasManager.SetTime("white", newTime.Value.ToString());
            
        } else {
            if(IsHost) {
                calculateBlackTimeServerRpc(false);
            }
            canvasManager.SetTime("black", newTime.Value.ToString());
        }

        if(!IsHost)
            return; 

        if(whiteTime.Value < 0f) {
            calculateWhiteTimeServerRpc(true);
            controller.GetComponent<Game>().WinnerServerRpc("black");
            enabled = false;
        } else if (blackTime.Value < 0f) {
            calculateBlackTimeServerRpc(true);
            controller.GetComponent<Game>().WinnerServerRpc("white");
            enabled = false;
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        if (timeInSeconds < 0) {
            timeInSeconds = 0f;
        }

        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    [ServerRpc]
    private void calculateWhiteTimeServerRpc(bool timeIsUp) {
        if (timeIsUp) {
            whiteTime.Value = 0f;
        } else {
            whiteTime.Value -= Time.deltaTime;
        }
        
        newTime.Value = FormatTime(whiteTime.Value);
    }
    
    [ServerRpc]
    private void calculateBlackTimeServerRpc(bool timeIsUp) {
        if (timeIsUp) {
            blackTime.Value = 0f;
        } else {
            blackTime.Value -= Time.deltaTime;
        }

        newTime.Value = FormatTime(blackTime.Value);
    }
}
