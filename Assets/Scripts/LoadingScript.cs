using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string userId = PlayerPrefs.GetString("userId");

        if (userId == "") 
        {
            SceneManager.LoadScene("SignUpScene");
        } 
        else
        {
            SceneManager.LoadScene("MainMenuScene");
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
