using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button signUpButton;
    public DatabaseManager databaseManager;

    private FirebaseAuth auth;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;

            if (app == null || auth == null)
            {
                Debug.LogError($"Failed to initialize Firebase with {task.Exception}");
            }
        });

        loginButton.onClick.AddListener(Login);
        signUpButton.onClick.AddListener(GoToSignUpScene);
    }

    private void Login()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Failed to sign in with {task.Exception}");
                // You might want to display an error message to the user
                return;
            }

            FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

            if (user != null) 
            {
                Debug.Log("User signed in successfully!");
            
                // Access user properties
                string userId = user.UserId;
                Debug.Log($"User ID: {userId}");

                PlayerPrefs.SetString("userId", userId);
                PlayerPrefs.Save();

                databaseManager.SaveUserInfoLocallyFromDatabase(userId, OnDatabaseOperationComplete);
            }
            else
            {
                Debug.LogError("User is null!");
            }
        });
    }

    private void OnDatabaseOperationComplete()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void GoToSignUpScene()
    {
        // Add code here to transition to your sign-up scene
        // For example, you can use SceneManager.LoadScene("SignUpScene");
        SceneManager.LoadScene("SignUpScene");
    }
}
