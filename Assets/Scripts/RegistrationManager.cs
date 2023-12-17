using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;

public class RegistrationManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public Button signUpButton;
    public Button loginButton;
    public GameObject loadingCircle;

    private FirebaseAuth auth;
    private FirebaseFirestore firestoreDatabase;

    private void Awake()
    {
        loadingCircle.gameObject.SetActive(false);
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
        loadingCircle.gameObject.SetActive(true);
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (password != confirmPassword)
        {
            Debug.LogError("Passwords do not match.");
            // You might want to display an error message to the user
            return;
        }

        // Continue with user registration
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Failed to create user with {task.Exception}");
                // You might want to display a generic error message to the user
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
}
