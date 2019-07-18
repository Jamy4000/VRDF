using UnityEngine;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// 
    /// A version of Unity's baked navmesh that is converted to a (serializable) component.  This allows the navmesh 
    /// used for Vive navigation to be separated form the AI Navmesh.  ViveNavMesh also handles the rendering of the 
    /// NavMesh grid in-game.
    /// </summary>
    public class TeleportNavMeshAuthoring : MonoBehaviour
    {
        [SerializeField] public int _QueryTriggerInteraction = 0;

        [SerializeField]
        [HideInInspector] public int NavAreaMask = ~0; // Initialize to all

        [SerializeField] public bool _IgnoreSlopedSurfaces = true;

        [SerializeField] public float _SampleRadius = 0.25f;

        #region GETTERS_SETTERS
        public float SampleRadius
        {
            get { return _SampleRadius; }
        }
        #endregion GETTERS_SETTERS
    }
}