using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public void SaveUserInfoLocallyFromDatabase(string userId, System.Action onDatabaseOperationComplete) {

        FirebaseApp app = FirebaseApp.DefaultInstance;
        FirebaseFirestore firestoreDatabase = FirebaseFirestore.GetInstance(app);

        DocumentReference docRef = firestoreDatabase.Collection("Users").Document(userId);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"Failed to fetch document snapshot: {task.Exception}");
                return;
            }

            DocumentSnapshot snapshot = task.Result;
        
            if (snapshot.Exists)
            {
                // Document exists, retrieve data
                Dictionary<string, object> data = snapshot.ToDictionary();

                // Access specific fields
                if (data.TryGetValue("username", out object username))
                {
                    // Do something with the field value
                    Debug.Log($"Field 'username' has value: {username}");
                    PlayerPrefs.SetString("username", username.ToString());
                    PlayerPrefs.Save();
                }
                else
                {
                    Debug.LogWarning("Field 'username' not found in the document.");
                }

                onDatabaseOperationComplete?.Invoke();

                // You can also create a class to represent your document data
                // For example, if your document has a 'name' field of type string:
                // string name = snapshot.GetValue<string>("name");

                // Now you can use the retrieved data as needed
            }
            else
            {
                Debug.LogWarning("Document does not exist.");
            }
        });
    }
}
