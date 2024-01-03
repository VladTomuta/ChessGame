using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class EloCalculator : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore firestoreDatabase;

    async void Start() {
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
    }

    public float Probability(float rating1, float rating2)
    {
        return 1.0f * 1.0f
            / (1
               + 1.0f
                     * (float)(Math.Pow(
                         10, 1.0f * (rating1 - rating2)
                                 / 400)));
    }
 
    // Function to calculate Elo rating
    // K is a constant.
    // d determines whether Player A wins or
    // Player B.
    public int[] EloRating(string userId1, string userId2, string username1, string username2, float Ra, float Rb, int K, bool d)
    {
 
        // To calculate the Winning
        // Probability of Player B
        float Pb = Probability(Ra, Rb);
 
        // To calculate the Winning
        // Probability of Player A
        float Pa = Probability(Rb, Ra);
 
        // Case -1 When Player A wins
        // Updating the Elo Ratings
        if (d == true) {
            Ra = Ra + K * (1 - Pa);
            Rb = Rb + K * (0 - Pb);
        }
 
        // Case -2 When Player B wins
        // Updating the Elo Ratings
        else {
            Ra = Ra + K * (0 - Pa);
            Rb = Rb + K * (1 - Pb);
        }
 
        Debug.Log("Updated Ratings:-\n");
 
        int roundedRa = (int)Math.Round(Ra);
        int roundedRb = (int)Math.Round(Rb);

        Debug.Log(
            "Ra = "
            + roundedRb
            + " Rb = "
            + roundedRb);

        int[] results = new int[2];
        results[0] = roundedRa;
        results[1] = roundedRb;

        return results;

        AddDocumentToCollection(
            userId1,
            username1,
            roundedRa
        );

        AddDocumentToCollection(
            userId2,
            username2,
            roundedRb
        );
    }

    public void AddDocumentToCollection(string userId, string newUsername, int startingRating)
    {
        // Get a reference to the collection
        CollectionReference userCollectionRef = firestoreDatabase.Collection("Users");

        Debug.Log("Collection ref aquired!");

        DocumentReference userDocumentRef = userCollectionRef.Document(userId);

        Debug.Log("Document ref aquired!");

        User user = new User
        {
            username = newUsername,
            rating = startingRating
        };

        userDocumentRef.SetAsync(user).ContinueWithOnMainThread(task => {
                Debug.Log("Added user data to the document in the Users collection.");
        });
    }
}
