using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Linq;

public class RegistrationManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public Button signUpButton;
    public Button loginButton;
    public GameObject loadingCircle;
    public TMP_Text errorMessage;

    private FirebaseAuth auth;
    private FirebaseFirestore firestoreDatabase;

    private void Awake()
    {
        loadingCircle.gameObject.SetActive(false);
        errorMessage.gameObject.SetActive(false);
    }

    private async void Start()
    {
        await FirebaseApp.CheckAndFixDependenciesAsync();
        FirebaseApp app = FirebaseApp.DefaultInstance;

        if (app == null)
        {
            Debug.LogError("Failed to initialize Firebase");
            return;
        }

        auth = FirebaseAuth.DefaultInstance;
        firestoreDatabase = FirebaseFirestore.DefaultInstance;

        if (auth == null || firestoreDatabase == null)
        {
            Debug.LogError("Failed to get Firebase instances");
            return;
        }

        if(PlayerPrefs.GetString("userId") != "") {
            SceneManager.LoadScene("MainMenuScene");
        }

        // Enable buttons only after both authentication and Firestore are initialized
        signUpButton.interactable = true;
        loginButton.interactable = true;

        Debug.Log("Firebase initialization successful");

        

        signUpButton.onClick.AddListener(SignUp);
        loginButton.onClick.AddListener(GoToLoginScene);
    }

    private void SignUp()
    {
        errorMessage.gameObject.SetActive(false);
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (string.IsNullOrEmpty(username)) {
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = "Username field is required";
            return;
        }

        if (string.IsNullOrEmpty(email)) {
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = "Email field is required";
            return;
        }

        if (string.IsNullOrEmpty(password)) {
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = "Password field is required";
            return;
        }

        if (string.IsNullOrEmpty(confirmPassword)) {
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = "Confirm password field is required";
            return;
        }

        if (password != confirmPassword)
        {
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = "Passwords do not match";
            return;
        }

        loadingCircle.gameObject.SetActive(true);

        // Continue with user registration
        CheckUsernameAvailability(username, isAvailable =>
        {
            if (isAvailable)
            {
                // Continue with user registration
                auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        // Handle registration failures and display appropriate error messages
                        Debug.Log(task.Exception);
                        HandleSignUpError(task.Exception);
                        return;
                    }

                    // Registration successful, you can now save additional user data to the database
                    string userId = task.Result?.User?.UserId;

                    Debug.Log("User registration successful!");

                    Debug.Log("Username: " + username);
                    Debug.Log("Email: " + email);

                    AddDocumentToCollection(userId, username);

                    PlayerPrefs.SetString("userId", userId);
                    PlayerPrefs.SetString("username", username);
                    PlayerPrefs.Save();

                    // Call a function to switch scenes or use Unity's SceneManager.LoadScene
                    SceneManager.LoadScene("MainMenuScene");
                });
            }
            else
            {
                errorMessage.gameObject.SetActive(true);
                errorMessage.text = "Username is already in use";
                loadingCircle.gameObject.SetActive(false);
            }
        });
    }

    private void GoToLoginScene()
    {
        // Add code here to transition to your login scene
        // For example, you can use SceneManager.LoadScene("LoginScene");
        SceneManager.LoadScene("LoginScene");
    }

    private void AddDocumentToCollection(string userId, string newUsername)
    {
        // Get a reference to the collection
        CollectionReference userCollectionRef = firestoreDatabase.Collection("Users");

        Debug.Log("Collection ref aquired!");

        DocumentReference userDocumentRef = userCollectionRef.Document(userId);

        Debug.Log("Document ref aquired!");

        User user = new User
        {
            username = newUsername
        };

        userDocumentRef.SetAsync(user).ContinueWithOnMainThread(task => {
                Debug.Log("Added user data to the document in the Users collection.");
        });
    }

    private void CheckUsernameAvailability(string username, Action<bool> callback)
    {
        // Get a reference to the Users collection
        CollectionReference userCollectionRef = firestoreDatabase.Collection("Users");

        // Query to check if the username is already in use
        Query query = userCollectionRef.WhereEqualTo("username", username);

        // Execute the query
        query.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"Failed to check username availability with {task.Exception}");
                callback(false); // Assume username is not available in case of error
                return;
            }

            bool isAvailable = task.Result.Documents.Count() == 0;
            callback(isAvailable);
        });
    }

    private void HandleSignUpError(AggregateException exception)
    {
        foreach (FirebaseException error in exception.Flatten().InnerExceptions)
        {
            string errorCode = error.ErrorCode.ToString();

            Debug.Log(errorCode);

            switch (error.ErrorCode)
            {
                case 11: //"ERROR_INVALID_EMAIL"
                    errorMessage.text = "Invalid email format";
                    break;
                case 8: //"ERROR_EMAIL_ALREADY_IN_USE"
                    errorMessage.text = "Email is already in use";
                    break;
                case 23: //RROR_WEAK_PASSWORD
                    errorMessage.text = "Password is too weak";
                    break;
                // Add more cases for other error codes as needed
                default:
                    errorMessage.text = "Registration failed";
                    break;
            }
        }

        errorMessage.gameObject.SetActive(true);
        loadingCircle.gameObject.SetActive(false);
    }
}
