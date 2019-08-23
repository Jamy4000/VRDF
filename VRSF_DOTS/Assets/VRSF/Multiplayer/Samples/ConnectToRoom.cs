using UnityEngine;

namespace VRSF.Multiplayer.Samples
{
    public class ConnectToRoom : MonoBehaviour
    {
        public string RoomName = "A Room Name";

        public void Connect()
        {
            new OnConnectionToRoomRequested(RoomName);
        }
    }
}