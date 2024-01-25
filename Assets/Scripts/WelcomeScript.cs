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
    public TMP_Dropdown gameMode;
    private string username;
    private string rating;

    // Start is called before the first frame update
    void Start()
    {
        username = PlayerPrefs.GetString("username");
        rating = PlayerPrefs.GetString("rating");

        welcomeMessage.text = "Welcome\n" + username;
        ratingMessage.text = rating + " elo";

        playGameButton.onClick.AddListener(PlayGame);
        logoutButton.onClick.AddListener(Logout);
    }

    private void PlayGame() {
        PlayerPrefs.SetString("gameMode", gameMode.options[gameMode.value].text);
        SceneManager.LoadScene("GameScene");
    }

    private void Logout() {

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        SceneManager.LoadScene("LoginScene");
    }
}
