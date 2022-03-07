// Source: https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/pun-basics-tutorial/lobby-ui

using UnityEngine;
using UnityEngine.UI;


using Photon.Pun;
using Photon.Realtime;


using System.Collections;

/// <summary>
/// Player name input field. Let the user input his name, will appear above the player in the game.
/// </summary>
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    #region Private Constants


    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";

    #endregion

    private InputField _inputField;
    private bool _inputFieldWasFocused;
    private TouchScreenKeyboard _keyboard;

    #region MonoBehaviour CallBacks


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start () {


        string defaultName = string.Empty;
        _inputField = this.GetComponent<InputField>();
        if (_inputField!=null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }


        PhotonNetwork.NickName =  defaultName;
    }

    private void Update()
    {
        bool inputFieldIsFocused = _inputField.isFocused;
        if (inputFieldIsFocused && !_inputFieldWasFocused)
        {
            _keyboard = TouchScreenKeyboard.Open("");
        }
        _inputFieldWasFocused = inputFieldIsFocused;
        if (_keyboard != null)
        {
            _inputField.text = _keyboard.text;
            _inputField.caretPosition = _inputField.text.Length;
            switch (_keyboard.status)
            {
                case TouchScreenKeyboard.Status.Visible:
                    break;
                case TouchScreenKeyboard.Status.Canceled:
                case TouchScreenKeyboard.Status.Done:
                case TouchScreenKeyboard.Status.LostFocus:
                    _inputField.DeactivateInputField();
                    break;
            }
        }
    }

    #endregion


    #region Public Methods


    /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;


        PlayerPrefs.SetString(playerNamePrefKey,value);
    }


    #endregion
}