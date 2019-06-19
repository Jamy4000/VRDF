using UnityEngine;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Contains the references for the Controllers Prefabs (The models)
    /// </summary>
    [System.Serializable]
    public struct VRController
    {
        [SerializeField] public EHand Hand;
        [SerializeField] public GameObject ControllerPrefab;
    }
}