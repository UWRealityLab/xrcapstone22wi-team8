using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR;
using System;

public class WhiteboardMarker : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    public int _penSize = 5;
    public FlexibleColorPicker fcp;
    
    private Vector3 _tipOrigin;
    private Renderer _renderer;
    private Color[] _colors;
    private float _tipHeight;

    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Vector2 _touchPos, _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;
    private Vector3 cast_pos;
    private InputDevice _rightController;
    private bool gotColor;
    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
        _tipHeight = _tip.localScale.y * 3 /4;
        _tipOrigin = _tip.localPosition;
        cast_pos = transform.up;

        var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);

        _rightController = rightHandedControllers[0];
    }

    void Update()
    {
        Draw();
    }

    private void Draw()
    {
        int layer_mask = LayerMask.GetMask("Whiteboard");
        if (Physics.Raycast(_tip.position, cast_pos, out _touch, _tipHeight*1.45f, layer_mask))
        {
            if(_touch.distance < _tipHeight*0.98) {
                if (_touch.transform.CompareTag("Whiteboard"))
                {
                    if (_whiteboard == null)
                    {
                        _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                    }
                    cast_pos = -_touch.normal;
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
                    // Debug.Log(_rightController.TryGetFeatureValue(CommonUsages.trigger, out float inputBool));
                    // Debug.Log(inputBool);
                    if (_rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool inputBool) && inputBool)
                    {
                        
                        _touchedLastFrame = false;
                        Color toSet = _whiteboard.texture.GetPixel(x, y);
                        updateColor(toSet);
                        return;
                    }
                    if (_touchedLastFrame)
                    {   
                        _whiteboard.texture.SetPixels(x, y, _penSize, _penSize, _colors);

                        for (float f = 0.005f; f < 1.00f; f += 0.005f)
                        {
                            var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                            var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                            _whiteboard.texture.SetPixels(lerpX, lerpY, _penSize, _penSize, _colors);
                        }

                            // transform.rotation = _lastTouchRot;
                        
                        _whiteboard.texture.Apply();
                    }

                    _lastTouchPos = new Vector2(x, y);
                     //_lastTouchRot = transform.rotation;
                    _touchedLastFrame = true;
                    return;
                }
            }
        }
        cast_pos = transform.up;
        _tip.transform.localPosition = _tipOrigin;
        _whiteboard = null;
        _touchedLastFrame = false;
    }

    public void updatePensize(float pensize) {
        _penSize = (int)pensize;
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
    }
    public void updateColor(Color newcolor) {
        _renderer.material.color = newcolor;
        _colors = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
    }

}