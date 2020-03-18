using Photon.Pun;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Used to display a different body for the remote and local users
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class VRDFPlayerModel : MonoBehaviourPun
    {
        [Header("Models for this PhotonView")]
        [Tooltip("The 3D Model to represent this photon view object if the user is a local user")]
        public GameObject LocalModel;

        [Tooltip("The 3D Model to represent this photon view object if the user is a remote user")]
        public GameObject RemoteModel;

        // Use this for initialization
        protected virtual void Start()
        {
            if (photonView.Owner.IsLocal && LocalModel != null)
                GameObject.Instantiate(LocalModel, transform);
            else if (!photonView.Owner.IsLocal && RemoteModel != null)
                GameObject.Instantiate(RemoteModel, transform);

            Destroy(this);
        }
    }
}