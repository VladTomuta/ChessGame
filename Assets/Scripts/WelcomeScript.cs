using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WelcomeScript : MonoBehaviour
{

    public TMP_Text welcomeMessage;
    public TMP_Text ratingMessage;
    public Button playGameButton;
    public Button logoutButton;
    private string username;
    private string rating;

    // Start is called before the first frame update
    void Start()
    {
        username = PlayerPrefs.GetString("username");
        rating = PlayerPrefs.GetString("rating");
        
        Debug.Log("The user's username is: " + username);
        welcomeMessage.text = "Welcome\n" + username;
        ratingMessage.text = rating + " elo";

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
