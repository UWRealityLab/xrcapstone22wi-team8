using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;

public class RecognizeText : MonoBehaviour
{
    [SerializeField] private GameObject textDisplay;

    private TesseractDriver _tesseractDriver;
    private Texture2D _texture;
    private InputDevice _rightController;
    private bool _enable = false;
    private GameObject _textBox;

    private void Start()
    {
        var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);

        _rightController = rightHandedControllers[0];
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
            if (_textBox is null)
            {
                _textBox = Instantiate(textDisplay, new Vector3(0, 2, 0), Quaternion.identity);
            }
            if (_rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool inputBool) && inputBool)
            {
                var whiteboard = this.transform.GetComponent<Whiteboard>(); 
                Texture2D originalTexture = whiteboard.texture;
                Texture2D texture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.ARGB32, false);
                texture.SetPixels32(originalTexture.GetPixels32());
                texture.Apply();

                _tesseractDriver = new TesseractDriver();
                Recoginze(texture);
            }
        }
    }

    private void Recoginze(Texture2D outputTexture)
    {
        _texture = outputTexture;

        ClearTextDisplay();
        _tesseractDriver.CheckTessVersion();
        _tesseractDriver.Setup(OnSetupCompleteRecognize);
    }

    private void OnSetupCompleteRecognize()
    {
        AddToTextDisplay(_tesseractDriver.Recognize(_texture));
        AddToTextDisplay(_tesseractDriver.GetErrorMessage(), true);
    }

    private void ClearTextDisplay()
    {
        if (_textBox is null)
        {
            _textBox = Instantiate(textDisplay, new Vector3(0, 2, 0), Quaternion.identity);
        } else
        {
            _textBox.GetComponent<UnityEngine.UI.Text>().text = "";
        }
    }

    private void AddToTextDisplay(string text, bool isError = false)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        _textBox.GetComponent<UnityEngine.UI.Text>().text += text;

        if (isError)
            Debug.LogError(text);
        else
            Debug.Log(text);
    }
}