using Photon.Pun;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Used to display a different body for the remote and local users
    /// </summary>
    [RequireComponent(typeof(PhotonView), typeof(VRDFPlayerManager))]
    public class VRDFPlayerModel : MonoBehaviourPun
    {
        [Header("The Body Model for this Player")]
        [Tooltip("The 3D Model to represent this photon view object if the user is a local user")]
        public GameObject LocalModel;

        [Tooltip("The 3D Model to represent this photon view object if the user is a remote user")]
        public GameObject RemoteModel;

        // Use this for initialization
        protected virtual void Start()
        {
            if (photonView.Owner.IsLocal && LocalModel != null)
                GetComponent<VRDFPlayerManager>().ThisPlayer.PlayerBodyModel = GameObject.Instantiate(LocalModel, transform);
            else if (!photonView.Owner.IsLocal && RemoteModel != null)
                GetComponent<VRDFPlayerManager>().ThisPlayer.PlayerBodyModel = GameObject.Instantiate(RemoteModel, transform);

            Destroy(this);
        }
    }
}