using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WhiteboardMarkerColorUpdating : MonoBehaviour
{
    [SerializeField] private WhiteboardMarker _whiteboardMarker;

    private InputDevice _rightController;
    private InputDevice _leftController;
    private bool _eraserMode = false;
    private Color _oldColor;
    private bool _lastPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        var rightHandedControllers = new List<InputDevice>();
        var desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);
        if (rightHandedControllers.Count > 0)
        {
            _rightController = rightHandedControllers[0];
        }
        else
        {
            if (!Application.isEditor)
            {
                Debug.LogError("No rightHandedController");
            }
        }

        desiredCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);
        if (rightHandedControllers.Count > 0)
        {
            _leftController = rightHandedControllers[0];
        }
        else
        {
            if (!Application.isEditor)
            {
                Debug.LogError("No leftHandedController");
            }
        }

        _oldColor = _whiteboardMarker.CurrentColor();
    }

    // Update is called once per frame
    void Update()
    {
        var lastBool = _lastPressed;
        if (!_leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out _lastPressed))
        {
            return;
        }
        if ((lastBool != _lastPressed) && _lastPressed)
        {
            if (_eraserMode)
            {
                _whiteboardMarker.updateColor(_oldColor);
                _oldColor = Color.white;
            }
            else
            {
                _oldColor = _whiteboardMarker.CurrentColor();
                _whiteboardMarker.updateColor(Color.white);
            }
            _eraserMode = !_eraserMode;
        }
    }

    public bool TriggerButtonPressed()
    {
        return _rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool inputBool) && inputBool;
    }
}
