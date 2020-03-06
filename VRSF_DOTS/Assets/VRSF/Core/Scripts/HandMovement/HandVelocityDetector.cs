using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

namespace VRSF.Core
{
    /// <summary>
    /// Detect the velocity and angular velocity of one of the user's hand
    /// </summary>
    public class HandVelocityDetector : MonoBehaviour
    {
        [Header("Checking left hand ?")]
        [SerializeField] private bool _isLeftHand;

        [Header("Output Variables")]
        /// <summary>
        /// The actual Velocity of the Hand
        /// </summary>
        [Tooltip("The actual velocity of the XR Node")]
        public Vector3 HandVelocity;

        /// <summary>
        /// The actual Angular Velocity of the Hand
        /// </summary>
        [Tooltip("The actual Angular Velocity of the XR Node")]
        public Vector3 HandAngularVelocity;

        void FixedUpdate()
        {
            List<XRNodeState> nodes = new List<XRNodeState>();
            InputTracking.GetNodeStates(nodes);

            foreach (XRNodeState ns in nodes)
            {
                if (_isLeftHand && ns.nodeType == XRNode.LeftHand)
                {
                    ns.TryGetVelocity(out HandVelocity);
                    ns.TryGetAngularVelocity(out HandAngularVelocity);
                }
                else if (!_isLeftHand && ns.nodeType == XRNode.RightHand)
                {
                    ns.TryGetVelocity(out HandVelocity);
                    ns.TryGetAngularVelocity(out HandAngularVelocity);
                }
            }
        }
    }
}