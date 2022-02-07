using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields
    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 10;
    #endregion

    #region Private Fields
    string gameVersion = "1";
    #endregion

    #region MonoBehaviour CallBacks

    void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master
        // client and all clients in the same room sync their level
        // automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide the "Connecting..." label until we're actually connecting
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    #endregion

    #region Public Fields
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;
    #endregion

    #region Public Methods
    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // check if we are connected or not, we join if we are, else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
    #endregion

    #region MonoBehaviourPunCallbacks Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conected to PUN");
        // #Critical: The first we try to do is to join a potential existing room.
        // If there is, good, else, we'll be called back with OnJoinRandomFailed()
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available, creating one");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN OnDisconnected() was called by PUN with reason {0}", cause);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    #endregion
}
