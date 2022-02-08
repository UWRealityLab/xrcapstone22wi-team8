using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Firestore;
using Firebase.Storage;
using UnityEngine;

public class FirebaseServices : MonoBehaviour
{
    private FirebaseApp _app = null;
    private FirebaseFirestore _db = null;
    private FirebaseStorage _storage = null;

    // FIXME: only works for room "test"
    public const string Room = "test";

    /// <summary>
    /// https://firebase.google.com/docs/firestore/query-data/listen#view_changes_between_snapshots
    /// </summary>
    public Action<QuerySnapshot> OnRoomDocumentsChange = snapshot =>
    {
        // FIXME: for DEBUG only, should replace with null or actual handler
        foreach (DocumentChange change in snapshot.GetChanges())
        {
            switch (change.ChangeType)
            {
                case DocumentChange.Type.Added:
                    Debug.Log($"Added {GetDisplayName(change.Document)}");
                    break;
                case DocumentChange.Type.Modified:
                    Debug.Log($"Modified {change.Document.Id}");
                    break;
                case DocumentChange.Type.Removed:
                    Debug.Log($"Removed {change.Document.Id}");
                    break;
            }
        }
    };
    private ListenerRegistration _listener = null;


    // Start is called before the first frame update
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                _app = Firebase.FirebaseApp.DefaultInstance;
                _db = FirebaseFirestore.DefaultInstance;
                _storage = FirebaseStorage.DefaultInstance;

                UnityEngine.Debug.Log("Firebase initialized");

                if (OnRoomDocumentsChange != null)
                {
                    _listener = _db.Collection(Room).Listen(snapshot =>
                    {
                        OnRoomDocumentsChange(snapshot);
                    });
                }
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void OnDestroy()
    {
        if (_listener != null)
        {
            _listener.Stop();
        }
    }

    public void AddText(string content, string name = null)
    {
        _db.Collection(Room).AddAsync(new Dictionary<string, object>
        {
            { "name", name },
            { "content", content }
        });
    }

    /// <summary>
    /// See https://firebase.google.com/docs/storage/unity/download-files on
    /// how to download the file contents.
    /// </summary>
    /// <param name="fileRef">Value for `ref` stored in database entry.</param>
    /// <returns>The storage reference for downloading the file.</returns>
    public StorageReference RefForDownload(string fileRef)
    {
        return _storage.GetReference($"{Room}/{fileRef}");
    }

    public static string GetDisplayName(DocumentSnapshot document)
    {
        string text;
        if (!document.TryGetValue("name", out text) || string.IsNullOrEmpty(text))
        {
            text = document.GetValue<string>("content");
        }
        return text;
    }
}
