using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseImageTexture : MonoBehaviour
{
    public Vector3 SpawnLocation = new Vector3(0, 2, 0);
    private FirebaseServices _firebase;

    // Start is called before the first frame update
    void Start()
    {
        TryLoadFirebaseServices();
    }

    [PunRPC]
    private void LoadImage(string name, string refOrNull, string contentOrNull, string extension) {
        StartCoroutine("LoadImageAsync", new OwO(new FirebaseFile(name, refOrNull, contentOrNull), extension));
    }

    private struct OwO
    {
        public FirebaseFile file;
        public string extension;

        public OwO(FirebaseFile file, string extension)
        {
            this.file = file;
            this.extension = extension;
        }
    }

    private IEnumerator LoadImageAsync(OwO owo)
    {
        FirebaseFile file = owo.file;
        string extension = owo.extension;
        Debug.Log(file.Name + ": " + file.RefOrNull);
        Debug.Log(extension);
        TryLoadFirebaseServices();
        // Note: Only JPG and PNG formats are supported.
        // https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestTexture.GetTexture.html
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_firebase.LocalURIForFile(file)))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(uwr.error);

                InstantiateTextDisplay($"Can't display {file.Name}");
            }
            else
            {
                if (gameObject is null)
                {
                    Debug.LogError("No gameObject");
                }
                MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                if (mr is null)
                {
                    Debug.LogError("No MeshRenderer");
                }
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
                gameObject.transform.localScale = new Vector3(x, y, 1);
                Debug.Log($"({x}, {y}) for {tex.width}, {tex.height}");
            }
        }
    }

    private void InstantiateTextDisplay(string text)
    {
        GameObject TextContainerPrefab = (GameObject)Resources.Load("Text Display");
        if (TextContainerPrefab == null)
        {
            if (!Application.isEditor)
            {
                Debug.LogError("FirebaseImageTexture No TextContainerPrefab");
            }
            Debug.Log(text);
            return;
        }
        GameObject textContainer = Instantiate(TextContainerPrefab, SpawnLocation, Quaternion.identity);
        textContainer.GetComponentInChildren<TextMesh>().text = text;
    }

    private void TryLoadFirebaseServices()
    {
        Debug.Log("Try Load Firebase Services");
        if (_firebase is null)
        {
            _firebase = FindObjectOfType<FirebaseServices>();
            if (_firebase is null)
            {
                Debug.LogError("No FirebaseServices");
            }
        }
    }
}
