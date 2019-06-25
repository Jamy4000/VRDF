using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// A component to add an End Point feature on the pointer.
    /// Usefull as sometimes the laser can be quite small and it can be difficult to see where the user is pointing at.
    /// </summary>
    public class LaserPointerEndPoint : MonoBehaviour
    {
        public RaycastHitVariable HitVariable;
    }
} 