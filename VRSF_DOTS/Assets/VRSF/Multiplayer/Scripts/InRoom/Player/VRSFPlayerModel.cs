using Photon.Pun;
using UnityEngine;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Used to display a different body for the remote and local users
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class VRSFPlayerModel : MonoBehaviourPun
    {
        [Header("Models for this PhotonView")]
        [Tooltip("The 3D Model to represent this photon view object if the user is a local user")]
        [SerializeField]
        private GameObject _localModel;

        [Tooltip("The 3D Model to represent this photon view object if the user is a remote user")]
        [SerializeField]
        private GameObject _remoteModel;

        // Use this for initialization
        void Start()
        {
            if (photonView.Owner.IsLocal && _localModel != null)
                GameObject.Instantiate(_localModel, transform);
            else if (!photonView.Owner.IsLocal && _remoteModel != null)
                GameObject.Instantiate(_remoteModel, transform);

            Destroy(this);
        }
    }
}