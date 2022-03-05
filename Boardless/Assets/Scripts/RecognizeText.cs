using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;

public class RecognizeText : MonoBehaviour
{
    [SerializeField] private GameObject textDisplay;
    [SerializeField] private FirebaseServices firebase;

    private TesseractDriver _tesseractDriver;
    private Texture2D _texture;
    private InputDevice _rightController;
    private bool _enable = false;
    private GameObject _textBox;
    private bool _isSecondaryButtonPressed = false;

    private void Start()
    {
        var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);

        _rightController = rightHandedControllers[0];
        _tesseractDriver = new TesseractDriver();
        _tesseractDriver.CheckTessVersion();
        _tesseractDriver.Setup(null);
    }

    // Enables scaling. Called when object starts selected.
    public void ActivateOCR()
    {
        _enable = true;
    }

    // Disables scaling. Called when object exits selected.
    public void DeactivatOCR()
    {
        _enable = false;
    }

    void FixedUpdate()
    {
        if (_enable)
        {
            var oldValue = _isSecondaryButtonPressed;
            if (_rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out _isSecondaryButtonPressed))
            {
                if ((_isSecondaryButtonPressed != oldValue) && _isSecondaryButtonPressed)
                {
                    var whiteboard = this.transform.GetComponent<Whiteboard>();
                    Recoginze(whiteboard.texture);
                }
            }
        }
    }

    private void Recoginze(Texture2D outputTexture)
    {
        _texture = outputTexture;

        ClearTextDisplay();
        AddToTextDisplay(_tesseractDriver.Recognize(_texture));
        AddToTextDisplay(_tesseractDriver.GetErrorMessage(), true);
    }

    private void ClearTextDisplay()
    {
        if (_textBox is null) return;
        _textBox.GetComponentInChildren<TextMesh>().text = "";
    }

    private void AddToTextDisplay(string text, bool isError = false)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        if (isError)
        {
            Debug.LogError($"Recognization failed: {text}");
        }
        else
        {
            Debug.Log($"Recognized {text}");
            firebase.AddText(text);
        }

        if (_textBox is null)
        {
            if (textDisplay is null)
            {
                if (!Application.isEditor)
                {
                    Debug.LogError("RecognizeText no textDisplay");
                }
                return;
            }

            _textBox = Instantiate(textDisplay, new Vector3(0, 2, 0), Quaternion.identity);
            if (_textBox is null)
            {
                Debug.LogError("RecognizeText cannot duplicate textDisplay");
                return;
            }
            ClearTextDisplay();
        }
        _textBox.GetComponentInChildren<TextMesh>().text += text;
    }
}