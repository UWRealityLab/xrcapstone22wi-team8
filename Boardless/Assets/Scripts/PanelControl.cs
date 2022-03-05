using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class PanelControl : MonoBehaviour
{
    public GameObject Panel;
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
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }

    void FixedUpdate()
    {
        var lastBool = _lastPressed;
        if (!_rightController.TryGetFeatureValue(CommonUsages.primaryButton, out _lastPressed)) {
            return;
        }
        if ((lastBool != _lastPressed) && _lastPressed) {
            TogglePanel();
        }
    }
}
