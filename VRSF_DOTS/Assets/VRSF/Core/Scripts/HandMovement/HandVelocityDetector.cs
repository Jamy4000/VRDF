using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

namespace VRSF.Core
{
    public class HandVelocityDetector : MonoBehaviour
    {
        [SerializeField] private bool _isLeftHand;

        public Vector3 HandVelocity;
        public Vector3 HandAngularVelocity;

        // Update is called once per frame
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