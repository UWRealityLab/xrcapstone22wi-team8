using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviour 
{ 

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    private PhotonView photonView;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Get a reference to the XR origin and connect head and hand tracked objects
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        headRig = xrOrigin.transform.Find("Camera Offset/Main Camera");
        rightHandRig = xrOrigin.transform.Find("Camera Offset/RightHand Controller");
        leftHandRig = xrOrigin.transform.Find("Camera Offset/LeftHand Controller");

        // Don't display the network avatar version of yourself
        if (photonView.IsMine)
        {
            foreach(var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // Only update positions if this is YOUR avatar
            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);
        }
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
