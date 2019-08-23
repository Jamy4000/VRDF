using System;
using System.Collections.Generic;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Controllers
{
    public class ControllerMeshListing : MonoBehaviour
    {
        [Header("The Controller for this hand")]
        [Tooltip("This component group all controller's mesh for each loadable device. The created Controller will be set as child of the parent from the corresponding hand.")]
        [SerializeField] private VRControllers[] _vrControllers;

        public Dictionary<EDevice, GameObject> ControllersPerDevice = new Dictionary<EDevice, GameObject>();

        private void Awake()
        {
            foreach (var vrController in _vrControllers)
            {
                ControllersPerDevice.Add(vrController.ControllersDevice, vrController.ControllersMeshPrefabs);
            }
        }
    }


    [Serializable]
    public struct VRControllers
    {
        public EDevice ControllersDevice;
        public GameObject ControllersMeshPrefabs;
    }
}