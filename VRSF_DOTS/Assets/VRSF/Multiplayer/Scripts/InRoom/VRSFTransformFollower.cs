using UnityEngine;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Follow the ToFollow transform everywhere
    /// </summary>
    public class VRSFTransformFollower : MonoBehaviour
    {
        /// <summary>
        /// The Object's transform to follow
        /// </summary>
        public Transform ToFollow; 

        // Update is called once per frame
        void FixedUpdate()
        {
            if (ToFollow != null) 
                this.transform.SetPositionAndRotation(ToFollow.position, ToFollow.rotation);
        }
    }
}