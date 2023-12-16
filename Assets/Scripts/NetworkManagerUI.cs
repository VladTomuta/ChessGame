using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{

    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;
    [SerializeField] TMP_InputField joinCode;
    [SerializeField] GameObject chessRelay;

    private TMP_Text debugText;

    private GameObject controller;
    
    private void Awake() {

        controller = GameObject.FindGameObjectWithTag("GameController");

        hostButton.onClick.AddListener(async () => {
            //GameObject gameObject = GameObject.FindGameObjectWithTag("Loading");
            //Text text = gameObject.GetComponent<Text>();
            //text.enabled = true;
            //Debug.Log("Am apasat butonul");
            //NetworkManager.Singleton.StartHost();
            //GameObject.FindGameObjectWithTag("Loading").GetComponent<Text>().enabled = false;
            debugText = GameObject.FindGameObjectWithTag("Debug").GetComponent<TMP_Text>();
            ChessRelay chessRelayScript = chessRelay.GetComponent<ChessRelay>();
            await chessRelayScript.StartHostWithRelay(1);
        });
        clientButton.onClick.AddListener(async () => {
            GameObject.FindGameObjectWithTag("Loading").GetComponent<Text>().enabled = true;
            //NetworkManager.Singleton.StartClient();

            //StartCoroutine(WaitForThreeSeconds());

            //Debug.Log("Incepem initul");
            ChessRelay chessRelayScript = chessRelay.GetComponent<ChessRelay>();
            await chessRelayScript.StartClientWithRelay(joinCode.text);
            Debug.Log("Uite acum initializam piesele");
            StartCoroutine(WaitForThreeSeconds());
        });
    }

    IEnumerator WaitForThreeSeconds()
    {
        Debug.Log("Am intrat in functie!");
        debugText.text = "Am intrat in functie!";
        if (NetworkManager.Singleton.IsConnectedClient.ToString() == "False") {
            Debug.Log("goood progress");
            debugText.text = "goood progress";
        }

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);
        Debug.Log(NetworkManager.Singleton.IsConnectedClient.ToString());
        debugText.text = NetworkManager.Singleton.IsConnectedClient.ToString();

        if (NetworkManager.Singleton.IsConnectedClient.ToString() != "False") {
            Debug.Log("this is it");
            debugText.text = "this is it";
        }

        //Debug.Log(NetworkManager.Singleton.IsConnectedClient);

        // Code to execute after waiting for 3 seconds
        Debug.Log("Three seconds have passed!");
        //debugText.text = "Three seconds have passed!";
        controller.GetComponent<Game>().InitializePiecesServerRpc();
        GameObject.FindGameObjectWithTag("Loading").GetComponent<Text>().enabled = false;
    }
}
