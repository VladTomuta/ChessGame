using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanup : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake() {
        if (NetworkManager.Singleton != null) {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (ChessLobby.Instance != null) {
            Destroy(ChessLobby.Instance.gameObject);
        }

        if (ChessRelay.Instance != null) {
            Destroy(ChessRelay.Instance.gameObject);
        }
    }
}
