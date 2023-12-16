using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Button ResignButton;
    [SerializeField] private TMP_Text LoadingText;
    [SerializeField] private GameObject LoadingBackground;
    
    private bool opponentFound;

    private const float lookingForOpponentsUpdateMax = 0.5f;
    private float lookingForOpponentsUpdate;
    private int numberOfDotsAfterText;


    void Awake()
    {
        ResignButton.gameObject.SetActive(false);
        lookingForOpponentsUpdate = lookingForOpponentsUpdateMax;
        numberOfDotsAfterText = 0;
        opponentFound = false;
    }

    
    void Update()
    {
        if (opponentFound)
            return;
        lookingForOpponentsUpdate -= Time.deltaTime;
        if (lookingForOpponentsUpdate < 0f) {
            lookingForOpponentsUpdate = lookingForOpponentsUpdateMax;
            LoadingText.text = "Looking for opponent";
            if(numberOfDotsAfterText < 3) {
                numberOfDotsAfterText++;
                for (int i = 0; i < numberOfDotsAfterText; i++) {
                    LoadingText.text = LoadingText.text + ".";
                }
            } else {
                numberOfDotsAfterText = 0;
            }
        }
    }

    public void setText(string text) {
        LoadingText.text = text;
    }
}
