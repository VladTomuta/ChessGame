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

    private GameObject controller;
    
    private void Awake() {

        controller = GameObject.FindGameObjectWithTag("GameController");

        hostButton.onClick.AddListener(() => {
            GameObject gameObject = GameObject.FindGameObjectWithTag("Loading");
            Text text = gameObject.GetComponent<Text>();
            text.enabled = true;
            Debug.Log("Am apasat butonul");
            NetworkManager.Singleton.StartHost();
            GameObject.FindGameObjectWithTag("Loading").GetComponent<Text>().enabled = false;
            //controller.GetComponent<Game>().InitializePieces();
            //controller.GetComponent<Game>().InitializePiecesServerRpc();
        });
        clientButton.onClick.AddListener(() => {
            GameObject.FindGameObjectWithTag("Loading").GetComponent<Text>().enabled = true;
            NetworkManager.Singleton.StartClient();

            StartCoroutine(WaitForThreeSeconds());

            Debug.Log("Incepem initul");

            //while(NetworkManager.Singleton.IsConnectedClient.ToString() == "False");     

            //controller.GetComponent<Game>().InitializePiecesServerRpc();
        });
    }

    IEnumerator WaitForThreeSeconds()
    {
        if (NetworkManager.Singleton.IsConnectedClient.ToString() == "False") {
            Debug.Log("goood progress");
        }

        // Wait for 3 seconds
        yield return new WaitForSeconds(1f);
        Debug.Log(NetworkManager.Singleton.IsConnectedClient.ToString());

        if (NetworkManager.Singleton.IsConnectedClient.ToString() != "False") {
            Debug.Log("this is it");
        }

        //Debug.Log(NetworkManager.Singleton.IsConnectedClient);

        // Code to execute after waiting for 3 seconds
        Debug.Log("Three seconds have passed!");
        controller.GetComponent<Game>().InitializePiecesServerRpc();
        GameObject.FindGameObjectWithTag("Loading").GetComponent<Text>().enabled = false;
    }
    
}
