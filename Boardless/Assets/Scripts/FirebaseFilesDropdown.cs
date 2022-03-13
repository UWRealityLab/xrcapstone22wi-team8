using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Firebase.Firestore;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class FirebaseFilesDropdown : MonoBehaviour
{
    private Dropdown _dropDown;
    public FirebaseServices Firebase;
    public Vector3 SpawnLocation;
    public GameObject TextContainerPrefab;

    // document id -> file
    private SortedList<string, FirebaseFile> _documents = new SortedList<string, FirebaseFile>();
    private struct FirebaseFile
    {
        public string Name;
        public string RefOrNull;
        public string ContentOrNull;
    }

    private void Awake()
    {
        _dropDown = GetComponent<Dropdown>();
        _dropDown.ClearOptions();
        _dropDown.options.Add(new Dropdown.OptionData { text = "Select document to add" });
    }

    // Start is called before the first frame update
    void Start()
    {
        Firebase.OnRoomDocumentsChange = OnRoomDocumentsChange;
        _dropDown.onValueChanged.AddListener(delegate
        {
            OnDropdownValueChange(_dropDown);
        });
        string directoryPath = LocalDirectory();
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    private void OnRoomDocumentsChange(QuerySnapshot snapshot)
    {
        foreach (DocumentChange change in snapshot.GetChanges())
        {
            string id = change.Document.Id;
            int index;
            FirebaseFile file;
            switch (change.ChangeType)
            {
                case DocumentChange.Type.Added:
                    _documents[id] = FileForDocument(change.Document);
                    index = _documents.Keys.IndexOf(id) + 1;
                    file = FileForDocument(change.Document);
                    _dropDown.options.Insert(index, new Dropdown.OptionData { text = file.Name });
                    // need to aggreesively download for 3D object loading
                    DownloadIfFileRef(file, false);
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
                    DownloadIfFileRef(file, true);
                    break;
                case DocumentChange.Type.Removed:
                    index = _documents.Keys.IndexOf(id) + 1;
                    _dropDown.options.RemoveAt(index);
                    _documents.Remove(id);
                    // we cannot delete file because it can still be in use
                    break;
            }
        }
    }

    private FirebaseFile FileForDocument(DocumentSnapshot document)
    {
        string refOrNull, contentOrNull;
        document.TryGetValue("ref", out refOrNull);
        document.TryGetValue("content", out contentOrNull);
        return new FirebaseFile
        {
            Name = FirebaseServices.GetDisplayName(document),
            RefOrNull = refOrNull,
            ContentOrNull = contentOrNull,
        };
    }

    private string LocalDirectory()
    {
        return Path.Combine(Application.persistentDataPath, FirebaseServices.Room);
    }

    private string LocalPathForFile(FirebaseFile file)
    {
        if (file.RefOrNull == null)
        {
            return null;
        }
        else
        {
            return Path.Combine(LocalDirectory(), file.RefOrNull);
        }
    }

    private string LocalURIForFile(FirebaseFile file)
    {
        string path = LocalPathForFile(file);
        if (path == null)
        {
            return null;
        }
        else
        {
            return new Uri(path).AbsoluteUri;
        }
    }

    private void DownloadIfFileRef(FirebaseFile file, bool downloadIfExists)
    {
        string path = LocalPathForFile(file);
        if (path == null)
        {
            return;
        }
        if (File.Exists(path) && !downloadIfExists)
        {
            Debug.Log($"File exists at {path} and no download required.");
            return;
        }
        Firebase.RefForDownload(file.RefOrNull)
            // Firebase expects starting with file://
            .GetFileAsync(LocalURIForFile(file))
            .ContinueWith(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log($"File download OK: {path}.");
                }
                else
                {
                    Debug.Log($"File download NO: {path}.");
                }
            });
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
        FirebaseFile file = _documents[_documents.Keys[fileIndex]];
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
        if (TextContainerPrefab == null)
        {
            if (!Application.isEditor)
            {
                Debug.LogError("FirebaseFileDropdown No TextContainerPrefab");
            }
            Debug.Log(text);
            return;
        }
        GameObject textContainer = Instantiate(TextContainerPrefab, SpawnLocation, Quaternion.identity);
        textContainer.GetComponentInChildren<TextMesh>().text = text;
    }

    private void InstantiateFile(FirebaseFile file)
    {
        string path = LocalPathForFile(file);
        if (path == null)
        {
            Debug.LogError($"Trying to instantiate ${file.Name} without file in storage");
            return;
        }
        if (!File.Exists(path))
        {
            Debug.LogWarning($"{file.Name} isn't downloaded to ${path}");
            InstantiateTextDisplay($"{file.Name} is not available at this time");
            return;
        }
        string extension = Path.GetExtension(path).ToLower();
        // FIXME: implement handling per type.
        // Please arrange file extensions alphabetically to reduce merge conflicts.
        Debug.Log($"Trying to display {file.Name} at {path} with extension {extension}");
        switch (extension)
        {
            case ".jpeg":
            case ".jpg":
            case ".png":
                StartCoroutine(LoadImage(file, extension));
                return;
            case ".obj":
                GameObject newObject = new Dummiesman.OBJLoader().Load(path);
                if (newObject == null) break;
                newObject.transform.position = SpawnLocation;

                Vector3 size = GetObjBounds(newObject).size;
                Debug.Log(size);

                if (size == Vector3.zero)
                {
                    // not working
                    break;
                }

                BoxCollider collider = newObject.AddComponent<BoxCollider>();
                collider.size = size;

                // scale to about 0.5 so not too big not to small
                // https://stackoverflow.com/a/31670874
                float minSize = Math.Min(size.x, Math.Min(size.y, size.z));
                Vector3 newScale = newObject.transform.localScale * (0.5f / minSize);
                // box collider want positive, fine
                newScale.x = Math.Abs(newScale.x);
                newScale.y = Math.Abs(newScale.y);
                newScale.z = Math.Abs(newScale.z);

                newObject.transform.localScale = newScale;
                Debug.Log(newScale);

                AddXRInteractions(newObject);

                return;
            default:
                break;
        }
        InstantiateTextDisplay($"Can't display {file.Name}");
        return;
    }

    // https://answers.unity.com/questions/208645/true-scale-of-a-visible-gameobject-with-many-child.html
    private Bounds GetObjBounds(GameObject newObject)
    {
        Bounds bounds = new Bounds(newObject.transform.position, Vector3.zero);
        foreach (Renderer r in newObject.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }

    private void AddXRInteractions(GameObject newObject)
    {
        if (Application.isEditor)
        {
            Debug.Log("No XR interaction in Unity Editor");
            return;
        }
        MoveWithController moveWithController = newObject.AddComponent<MoveWithController>();
        moveWithController.InputScale = 0.3f;
        ChangeScale changeScale = newObject.AddComponent<ChangeScale>();
        changeScale.InputScale = 0.3f;

        XRSimpleInteractable interactable = newObject.AddComponent<XRSimpleInteractable>();
        interactable.selectMode = InteractableSelectMode.Single;
        interactable.selectEntered.AddListener((arg0) =>
        {
            moveWithController.ActivateMove();
            changeScale.ActivateScaling();
        });
        interactable.selectExited.AddListener((arg0) =>
        {
            moveWithController.DeactivateMove();
            changeScale.DeactivatScaling();
        });

        // FIXME: implement sync over Photon
    }

    private IEnumerator LoadImage(FirebaseFile file, string extension)
    {
        // Note: Only JPG and PNG formats are supported.
        // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestTexture.GetTexture.html
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(LocalURIForFile(file)))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(uwr.error);
                InstantiateTextDisplay($"Can't display {file.Name}");
            }
            else
            {
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = SpawnLocation;

                MeshRenderer mr = quad.GetComponent<MeshRenderer>();
                if (extension == ".png")
                {
                    // it could be transparent, so allow it
                    mr.material = new Material(Shader.Find("Unlit/Transparent Cutout"));
                }

                Texture2D tex = DownloadHandlerTexture.GetContent(uwr);

                // https://answers.unity.com/questions/1280514/how-to-make-gameobject-match-the-source-textures-s.html
                mr.material.mainTexture = tex;
                float x, y;
                if (tex.height < tex.width)
                {
                    y = 0.5f;
                    x = y / tex.height * tex.width;
                }
                else
                {
                    x = 0.5f;
                    y = x / tex.width * tex.height;
                }
                quad.transform.localScale = new Vector3(x, y, 1);
                Debug.Log($"({x}, {y}) for {tex.width}, {tex.height}");

                AddXRInteractions(quad);
            }
        }
    }
}