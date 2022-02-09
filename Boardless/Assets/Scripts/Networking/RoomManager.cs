using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


public class RoomManager : MonoBehaviourPunCallbacks
{

    #region Public Fields
    [Tooltip("The scene to load once connected to a multi-user server")]
    [SerializeField]
    private string multiuserScene;
    #endregion

    #region Photon Callbacks


    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0); // First scene in build, i.e. the waiting room
    }


    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }


    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }


    #endregion


    #region Public Methods
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Private Methods
    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Photon: Trying to Load a level but we are not the master Client");
        }

        Debug.LogFormat("Photon: Loading Level : {0}", multiuserScene);
        PhotonNetwork.LoadLevel(multiuserScene);
    }
    #endregion
}
