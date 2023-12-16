using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Button cancelButton;

    void Start() {
        cancelButton.onClick.AddListener(Cancel);
    }

    void Cancel() {
        SceneManager.LoadScene("MainMenuScene");
    }

}
