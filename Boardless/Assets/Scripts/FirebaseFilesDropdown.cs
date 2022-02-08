using System.Collections;
using System.Collections.Generic;
using Firebase.Firestore;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseFilesDropdown : MonoBehaviour
{
    private Dropdown _dropDown;
    public FirebaseServices Firebase;
    public Vector3 SpawnLocation;
    public GameObject TextContainerPrefab;

    // document id -> file
    private SortedList<string, File> _documents = new SortedList<string, File>();
    private struct File {
        public string Name;
        public string RefOrNull;
        public string ContentOrNull;
    }

    // Start is called before the first frame update
    void Start()
    {
        _dropDown = GetComponent<Dropdown>();
        _dropDown.ClearOptions();
        _dropDown.options.Add(new Dropdown.OptionData { text = "Select document to add" });

        Firebase.OnRoomDocumentsChange = OnRoomDocumentsChange;
        _dropDown.onValueChanged.AddListener(delegate
        {
            OnDropdownValueChange(_dropDown);
        });
    }

    private void OnRoomDocumentsChange(QuerySnapshot snapshot)
    {
        foreach (DocumentChange change in snapshot.GetChanges())
        {
            string id = change.Document.Id;
            int index;
            File file;
            switch (change.ChangeType)
            {
                case DocumentChange.Type.Added:
                    _documents[id] = FileForDocument(change.Document);
                    index = _documents.Keys.IndexOf(id) + 1;
                    file = FileForDocument(change.Document);
                    _dropDown.options.Insert(index, new Dropdown.OptionData { text = file.Name });
                    break;
                case DocumentChange.Type.Modified:
                    file = FileForDocument(change.Document);
                    _documents[id] = file;
                    index = _documents.Keys.IndexOf(id) + 1;
                    if (_dropDown.options[index].text != file.Name)
                    {
                        _dropDown.options.RemoveAt(index);
                        _dropDown.options.Insert(index, new Dropdown.OptionData { text = file.Name });
                    }
                    break;
                case DocumentChange.Type.Removed:
                    index = _documents.Keys.IndexOf(id) + 1;
                    _dropDown.options.RemoveAt(index);
                    _documents.Remove(id);
                    break;
            }
        }
    }

    private File FileForDocument(DocumentSnapshot document)
    {
        string refOrNull, contentOrNull;
        document.TryGetValue("ref", out refOrNull);
        document.TryGetValue("content", out contentOrNull);
        return new File
        {
            Name = FirebaseServices.GetDisplayName(document),
            RefOrNull = refOrNull,
            ContentOrNull = contentOrNull,
        };
    }

    private void OnDropdownValueChange(Dropdown dropdown)
    {
        int index = dropdown.value;
        // Special entry for no selection.
        if (index == 0)
        {
            return;
        }
        int fileIndex = index - 1;
        File file = _documents[_documents.Keys[fileIndex]];
        if (file.RefOrNull != null)
        {
            InstantiateFile(file);
        }
        else
        {
            InstantiateTextDisplay(file.ContentOrNull);
        }
        // Reset dropdown
        dropdown.value = 0;
    }

    private void InstantiateTextDisplay(string text)
    {
        GameObject textContainer = Instantiate(TextContainerPrefab, SpawnLocation, Quaternion.identity);
        textContainer.GetComponentInChildren<TextMesh>().text = text;
    }

    private void InstantiateFile(File file)
    {
        // FIXME: implement handling per type.
        // Please arrange file suffix alphabetically to reduce merge conflicts.
        StorageReference storageRef = Firebase.RefForDownload(file.RefOrNull);

        if (file.Name.EndsWith("*.png"))
        {
            InstantiateTextDisplay($"Can't display {file.Name}");
            return;
        }

        InstantiateTextDisplay($"Can't display {file.Name}");
    }
}
