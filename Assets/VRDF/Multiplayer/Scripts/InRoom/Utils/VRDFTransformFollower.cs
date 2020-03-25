using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Follow the ToFollow transform everywhere
    /// </summary>
    public class VRDFTransformFollower : MonoBehaviour
    {
        [Header("Optional : the Transform you want to move.")]
        [Tooltip("Let this at nulkl if you want to use the transform attached to this script")]
        [SerializeField] private Transform _toMoveAndRotate;

        /// <summary>
        /// The Object's transform to follow
        /// </summary>
        [Header("What we are following")]
        public Transform ToFollow;

        private void Awake()
        {
            if (_toMoveAndRotate == null)
                _toMoveAndRotate = transform;
        }

        void FixedUpdate()
        {
            if (ToFollow != null)
                _toMoveAndRotate.SetPositionAndRotation(ToFollow.position, ToFollow.rotation);
        }
    }
}