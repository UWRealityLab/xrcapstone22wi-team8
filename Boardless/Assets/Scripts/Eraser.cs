using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR;
using System;

public class Eraser : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private int _penSize = 5;

    private Renderer _renderer;
    private float _tipHeight;
    private Vector3 _tipOrigin;
    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Vector2 _touchPos;
    private InputDevice _leftController;
    private bool _lastPressed = false;
    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _tipHeight = _tip.localScale.y * 3 /4;
        _tipOrigin = _tip.localPosition;

        var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);
        _leftController = rightHandedControllers[0];
    }

    void Update()
    {
        Draw();
    }

    private void Draw()
    {
        var lastBool = _lastPressed;
        if (!_leftController.TryGetFeatureValue(CommonUsages.triggerButton, out _lastPressed)) {
            return;
        }
        int layer_mask = LayerMask.GetMask("Whiteboard");
        if (Physics.Raycast(_tip.position, transform.up, out _touch, _tipHeight*1.45f, layer_mask))
        {
            if (_touch.transform.CompareTag("Whiteboard"))
            {
                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }
                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);
                if (_touch.distance <= 0.015) {
                    var curr = _tip.transform.localPosition;
                    curr.y -= 0.008f;
                    _tip.transform.localPosition = curr;
                }
                //  else if (_touch.distance >= 0.025) {
                //     _tip.transform.localPosition = _tipOrigin;
                // }
                // Debug.Log(_touch.distance);
                // Debug.Log(_tip.localPosition);
                var x = (int)(_touchPos.x * _whiteboard.textureSize.x - (_penSize / 2));
                var y = (int)(_touchPos.y * _whiteboard.textureSize.y - (_penSize / 2));

                if (y < 0 || y > _whiteboard.textureSize.y || x < 0 || x > _whiteboard.textureSize.x) return;
                if ((lastBool != _lastPressed) && _lastPressed) {
                    Debug.Log("saving!");
                    byte[] bytes = _whiteboard.texture.EncodeToPNG();
                    var dirPath = Application.dataPath + "/../SaveImages/";
                    if(!System.IO.Directory.Exists(dirPath)) {
                        System.IO.Directory.CreateDirectory(dirPath);
                    }
                    System.IO.File.WriteAllBytes(dirPath + "Image" +System.DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")+ ".png", bytes);
                    return;
                }
                return;
            }
        }
        _whiteboard = null;
        _tip.transform.localPosition = _tipOrigin;
    }
    // public void updatePensize(float pensize) {
    //     _penSize = (int)pensize;
    //     _colors = Enumerable.Repeat(Color.white, _penSize * _penSize).ToArray();
    // }
}