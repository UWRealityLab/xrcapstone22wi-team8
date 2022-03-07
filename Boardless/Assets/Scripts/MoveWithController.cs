using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MoveWithController : MonoBehaviour
{
    // Rate for scaling down the input value from controller, in case too aggressive.
    public float InputScale;

    // Instruction about how to move the object
    public string Instruction = "Move with right controller. Use joystick to change distance";

    // The rotation of this object relative to the controller.
    private Quaternion _obj2controller;

    // The controller game object to query position and rotation.
    private GameObject _rightControllerGameObj;

    // Distance from controller to the object.
    private float _dist;

    private bool _moving = false;
    private GameObject _moveInstruction;
    private InputDevice _rightController;


    // Sets up the controller GameObject and InputDevice and gets the Rigidbody.
    void Awake()
    {
        _rightControllerGameObj = GameObject.Find("RightHand Controller");
        var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);

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
        _moveInstruction = GameObject.Find("MoveInstruction");
    }

    // Enables moving. Called when object starts selected.
    public void ActivateMove()
    {
        _moving = true;
        _moveInstruction.GetComponent<UnityEngine.UI.Text>().text = Instruction;
        _dist = (_rightControllerGameObj.transform.position - transform.position).magnitude;
        _obj2controller = Quaternion.Inverse(_rightControllerGameObj.transform.rotation) * transform.rotation;
    }

    // Disable moving. Called when object exits selected.
    public void DeactivateMove()
    {
        _moving = false;
        _moveInstruction.GetComponent<UnityEngine.UI.Text>().text = "";
    }

    // While moving is enabled, the object follows the right controller and the distance from the controller to the object
    // can be changed by the joystick. Any velocity of the object is set to zero in case of collision and movement.
    void FixedUpdate()
    {
        if (_moving && _rightController.TryGetFeatureValue(CommonUsages.grip, out float grip) && grip > 0) {
            if (_rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 inputValue))
            {
                _dist += inputValue[1] * InputScale;
            }
            transform.position = _rightControllerGameObj.transform.position + _rightControllerGameObj.transform.TransformVector(Vector3.forward * _dist);
            transform.rotation = _rightControllerGameObj.transform.rotation * _obj2controller;
        }
    }
}
