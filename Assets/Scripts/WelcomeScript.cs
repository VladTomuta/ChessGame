using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WelcomeScript : MonoBehaviour
{

    public TMP_Text welcomeMessage;
    public Button playGameButton;
    public Button logoutButton;
    private string username;

    // Start is called before the first frame update
    void Start()
    {
        //do 
        //{
            username = PlayerPrefs.GetString("username");
            Debug.Log("username = " + username);
        //}
        //while(username == "");
        
        Debug.Log("The user's username is: " + username);
        welcomeMessage.text = "Welcome\n" + username;

        playGameButton.onClick.AddListener(PlayGame);
        logoutButton.onClick.AddListener(Logout);
    }

    private void PlayGame() {
        //DISABLED DUE TO DEBUGGING
        SceneManager.LoadScene("GameScene");
        //SceneManager.LoadScene("LobbyScene");
    }

    private void Logout() {

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        SceneManager.LoadScene("LoginScene");
    }
}
