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
        [SerializeField] public int QueryTriggerInteraction = 0;

        [SerializeField] public int NavAreaMask = ~0; // Initialize to all

        [SerializeField] public bool IgnoreSlopedSurfaces = true;

        [SerializeField] public float SampleRadius = 0.25f;

        [SerializeField] public float SphereCastRadius = 0.1f;

        [SerializeField] public ENavmeshDewarpingMethod DewarpingMethod = ENavmeshDewarpingMethod.RoundToVoxelSize;

        [HideInInspector] public Mesh SelectableMesh;
    }
}