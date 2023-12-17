using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : NetworkBehaviour
{
    [SerializeField] private Button resignButton;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private GameObject loadingBackground;
    [SerializeField] private GameObject loadingCircle;

    private const float lookingForOpponentsUpdateMax = 0.5f;
    private float lookingForOpponentsUpdate;
    private int numberOfDotsAfterText;


    void Awake()
    {
        resignButton.gameObject.SetActive(false);
        lookingForOpponentsUpdate = lookingForOpponentsUpdateMax;
        numberOfDotsAfterText = 0;
    }

    public void setText(string text) {
        loadingText.text = text;
    }

    public void loadingIsDone() {
        Debug.Log("Hei distrugem si noi ceva?");
        resignButton.gameObject.SetActive(true); 
        loadingText.gameObject.SetActive(false);
        loadingBackground.SetActive(false);
        loadingCircle.gameObject.SetActive(false);

        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if(!IsHost) {
            camera.GetComponent<Transform>().rotation = new Quaternion(0,0, 180, 0);
        }
    }
}
