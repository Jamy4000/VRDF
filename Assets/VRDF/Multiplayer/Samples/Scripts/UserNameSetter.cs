using Photon.Pun;
using UnityEngine;

namespace VRDF.Multiplayer.Samples
{
    /// <summary>
    /// A simple example on how to set your User name with Photon.
    /// </summary>
    public class UserNameSetter : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string _defaultNickname = "VRDF Player";
        [SerializeField] private TMPro.TMP_InputField _nicknameInputField;

        private void Start()
        {
            PhotonNetwork.NickName = _defaultNickname;
            var placeHolderText = _nicknameInputField.placeholder as TMPro.TextMeshProUGUI;
            placeHolderText.text = _defaultNickname;
        }

        public void OnUserNameChanged(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                newName = _defaultNickname + " #" + Random.Range(0, 1000);
                Debug.Log("<b>[VRDF Sample]</b> The user name is empty, using default one and adding random number to it.\n Your current Nickname is " + newName);
            }

            PhotonNetwork.NickName = newName;
        }
    }
}