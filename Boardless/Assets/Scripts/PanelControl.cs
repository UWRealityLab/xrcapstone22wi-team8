using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class PanelControl : MonoBehaviour
{
    public GameObject Panel;
    public Camera Cam;
    public Vector3 localPos;

    private InputDevice _rightController;
    private bool _lastPressed = false;
    // Sets up the controller GameObject and InputDevice and gets the Rigidbody.
    void Awake()
    {
        var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);

        _rightController = rightHandedControllers[0];
        this.enabled = true;
    }

    public void TogglePanel()
    {
        if (Panel != null)
        {
            Panel.SetActive(!Panel.activeSelf);
        }
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        float dist = 3;
        if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, dist))
        {
            dist = hit.distance - (float)0.1;
        }
        localPos.z = dist;
        Panel.transform.localPosition = localPos;
        Panel.transform.eulerAngles = Cam.transform.eulerAngles;

        var lastBool = _lastPressed;
        if (!_rightController.TryGetFeatureValue(CommonUsages.primaryButton, out _lastPressed)) {
            return;
        }
        if ((lastBool != _lastPressed) && _lastPressed) {
            TogglePanel();
        }
    }
}
