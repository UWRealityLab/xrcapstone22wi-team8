using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Photon.Pun;
using Photon.Realtime;

// A SharedObject can be interacted with by anyone in the room. This is different than Photon's
// default paradigm where each object has only one controller. Thus, this class implements behavior
// to transfer owner
public class SharedObject : MonoBehaviourPunCallbacks
{
    #region Public Fields

    [Tooltip("The object that should be shared. It should have view ownership set to takeover")]
    public GameObject Object;

    #endregion

    #region Public Methods

    /// <summary>
    /// Takeover must be called on shared objects before attempting to control them (i.e.
    /// move/transform them).
    /// </summary>
    public void Takeover()
    {
        PhotonView ObjectView = Object.GetComponent<PhotonView>();
        Assert.IsNotNull(ObjectView, "Shared objects must be networked");
        Assert.AreEqual(ObjectView.OwnershipTransfer, OwnershipOption.Takeover,
                "Shared objects must have their ownership set to takeover");
        
        if (!ObjectView.IsMine)
        {
            ObjectView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
    }

    #endregion
}
