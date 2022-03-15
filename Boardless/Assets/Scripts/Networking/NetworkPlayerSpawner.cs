using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class NetworkPlayerSpawner : MonoBehaviour
{
    public PhotonView MyPhotonView;

    private GameObject spawnedPlayerPrefab;

    public void OnEnable()
    {
        Debug.Log("I joined room");
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
        MyPhotonView = spawnedPlayerPrefab.GetComponent<PhotonView>();
        if (MyPhotonView is null)
        {
            Debug.LogError("No MyPhotonView for self");
        }
    }

    public void OnDisable()
    {
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
