using System;
using System.Collections;

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

// NetworkedObjectGenerator generates 
public class NetworkedObjectGenerator : MonoBehaviour
{

    [Tooltip("The object to generate")]
    public GameObject Obj;

    [Tooltip("The initial position for the generated object")]
    public Vector3 Pos;

    #region Public Methods

    // [Tooltip("Generate the given object at the given position for all connected users")]
    public void Generate()
    {
        // This class is only for generating networked objects, so they must have a PhotonView
        UnityEngine.Assertions.Assert.IsNotNull(Obj.GetComponent<PhotonView>());
        
        Debug.LogFormat("NetworkedObjectGenerator.Generate: Generating a {0}", Obj.name);

        // TODO: Consider calling PhotonNetwork.InstantiateRoomObject() from the master client
        // instead, so that objects can outlive their creators in the room.
        // See: https://doc.photonengine.com/en-us/pun/current/gameplay/instantiation/
        PhotonNetwork.Instantiate(Obj.name, Pos, Quaternion.identity, 0);
    }

    #endregion
}
