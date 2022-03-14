using UnityEngine;
using Photon.Pun;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    public PhotonView MyPhotonView;

    private GameObject spawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("I joined room");
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);
        MyPhotonView = spawnedPlayerPrefab.GetComponent<PhotonView>();
        if (MyPhotonView is null)
        {
            Debug.LogError("No MyPhotonView for self");
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
