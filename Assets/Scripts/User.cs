using Firebase.Firestore;

[FirestoreData]
public struct User  
{
    [FirestoreProperty]
    public string username { get; set; }

    [FirestoreProperty]
    public int rating { get; set; }
}
