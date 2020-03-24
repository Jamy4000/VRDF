using Photon.Realtime;
using UnityEngine;

namespace VRDF.Multiplayer.Samples
{
    /// <summary>
    /// Simple sample script to create a room, and handle the "Create Room" button by checking the Photon network callbacks
    /// </summary>
    public class RoomCreator : Photon.Pun.MonoBehaviourPunCallbacks
    {
        [Tooltip("Button used to create a room")]
        [SerializeField] private UnityEngine.UI.Button _createRoomButton;

        [Tooltip("The name that would be given to a room by default")]
        [SerializeField] private string _defaultRoomName = "VRDF Test Room";

        [SerializeField] private TMPro.TMP_InputField _roomNameInputField;

        private string _currentRoomName;

        private void Awake()
        {
            OnVRDFRoomsListWasUpdated.Listeners += UpdateDisplayedRoomList;
            var placeHolderText = _roomNameInputField.placeholder as TMPro.TextMeshProUGUI;
            placeHolderText.text = _defaultRoomName;
            _currentRoomName = _defaultRoomName;
        }

        private void OnDestroy()
        {
            OnVRDFRoomsListWasUpdated.Listeners -= UpdateDisplayedRoomList;
        }

        /// <summary>
        /// Callback for the Create Room Button in the Sample Connection Scene
        /// </summary>
        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(_currentRoomName))
            {
                Debug.LogErrorFormat("<b>[VRDF Sample]</b> The room name is empty, using default one and adding random number to it.");
                VRDFConnectionManager.ConnectOrCreateRoom(_defaultRoomName + Random.Range(0, 1000), new RoomOptions { MaxPlayers = 5, PlayerTtl = 10000 });
            }
            else
            {
                VRDFConnectionManager.ConnectOrCreateRoom(_currentRoomName, new RoomOptions { MaxPlayers = 5, PlayerTtl = 10000 });
            }
        }

        /// <summary>
        /// Set the name of the room we want to create, using inputField onValueChanged callback
        /// </summary>
        /// <param name="newName"></param>
        public void SetRoomName(string newName)
        {
            _currentRoomName = newName;
            CheckCreateRoomButton();
        }

        /// <summary>
        /// Callback for when we receive an update on the list of available rooms
        /// </summary>
        /// <param name="info">The event containing the list of available rooms.</param>
        private void UpdateDisplayedRoomList(OnVRDFRoomsListWasUpdated _)
        {
            CheckCreateRoomButton();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            CheckCreateRoomButton();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            CheckCreateRoomButton();
        }

        /// <summary>
        /// Check if all requirement to create a room are reached, and set the interactibility of the connect button accordingly
        /// </summary>
        private void CheckCreateRoomButton()
        {
            if (Photon.Pun.PhotonNetwork.InLobby && RoomListFetcher.AvailableRooms.ContainsKey(_currentRoomName))
            {
                VRDF_Components.DebugVRDFMessage("Room with name {0} already exist, disabling create room button.", debugParams: _currentRoomName);
                _createRoomButton.interactable = false;
            }
            else if (!Photon.Pun.PhotonNetwork.IsConnectedAndReady)
            {
                VRDF_Components.DebugVRDFMessage("The user isn't connected to Game Server. Disabling create room button.");
                _createRoomButton.interactable = false;
            }
            else
            {
                _createRoomButton.interactable = true;
            }
        }
    }
}