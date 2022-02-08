﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;

// Uses to indicates which dimension to scale.
enum ScaleMode
{
    X,
    Y,
    Z,
    Overall
}

public class ChangeScale : MonoBehaviour
{
    // Rate for scaling down the input value from controller, in case too aggressive.
    public float InputScale;

    private bool _enable = false;
    private InputDevice _leftController;
    private ScaleMode _mode = ScaleMode.Overall;

    // Get the left controller as Input device.
    void Awake()
    {
        var leftHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, leftHandedControllers);

        _leftController = leftHandedControllers[0];
    }

    // Enables scaling. Called when object starts selected.
    public void ActivateScaling()
    {
        _enable = true;

    }

    // Disables scaling. Called when object exits selected.
    public void DeactivatScaling()
    {
        _enable = false;
    }

    void FixedUpdate()
    {
        if (_enable)
        {
            if (_leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool inputBool) && inputBool)
            {
                _mode = _mode == ScaleMode.Overall ? ScaleMode.X : _mode + 1;
            }

            Vector3 diff = Vector3.zero;
            if (_leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 inputValue))
            {
                // The inputValue[1] is the up-down movement, y-axis.
                float change = inputValue[1] * InputScale;
                if (_mode == ScaleMode.Overall)
                {
                    diff = Vector3.one * change;
                } else
                {
                    diff[(int)_mode] += change;
                }
            }
            transform.localScale = Vector3.Max(Vector3.zero, transform.localScale + diff);
        }
    }
}
