using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using Unity.VisualScripting;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button signUpButton;
    public DatabaseManager databaseManager;
    public GameObject loadingCircle;
    public TMP_Text errorMessage;

    private FirebaseAuth auth;

    private void Awake()
    {
        loadingCircle.gameObject.SetActive(false);
        errorMessage.gameObject.SetActive(false);
    }

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
        errorMessage.gameObject.SetActive(false);
        // if (string.IsNullOrEmpty(emailInput.text)) {
        //     errorMessage.gameObject.SetActive(true);
        //     errorMessage.text = "Email field is empty";
        //     //return;
        //     //37
        // }

        // if (string.IsNullOrEmpty(passwordInput.text)) {
        //     errorMessage.gameObject.SetActive(true);
        //     errorMessage.text = "Password field is empty";
        //     //38
        //     //return;
        // }

        loadingCircle.gameObject.SetActive(true);
        string email = emailInput.text;
        string password = passwordInput.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Failed to sign in with {task.Exception}");
                HandleSignInError(task.Exception);
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

    private void HandleSignInError(AggregateException exception)
    {
        foreach (FirebaseException error in exception.Flatten().InnerExceptions)
        {
            switch (error.ErrorCode)
            {
                case 37: //ERROR_MISSING_EMAIL
                    errorMessage.text = "Email field is empty";
                    break;
                case 38: //ERROR_MISSING_PASSWORD
                    errorMessage.text = "Password field is empty";
                    break;
                case 11: //ERROR_INVALID_EMAIL
                    //errorMessage.text = "Invalid email format";
                    //break;
                case 1: //ERROR_WRONG_PASSWORD || ERROR_USER_NOT_FOUND
                    errorMessage.text = "Incorrect email or password";
                    break;
                // Add more cases for other error codes as needed
                default:
                    errorMessage.text = "Authentication failed";
                    break;
            }
        }

        errorMessage.gameObject.SetActive(true);
        loadingCircle.gameObject.SetActive(false);
    }
}
