using System;
using System.Collections.Generic;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Provide a list of Meshes that can be instantiated based on the currently loaded VR Device.
    /// </summary>
    public class ControllerMeshesLister : MonoBehaviour
    {
        [Header("The Controller for this hand")]
        [Tooltip("This component group all controller's mesh for each loadable device. The created Controller will be set as child of the parent from the corresponding hand.")]
        [SerializeField] private VRControllers[] _vrControllers;

        public Dictionary<EDevice, VRControllers> ControllersPerDevice = new Dictionary<EDevice, VRControllers>();

        private void Awake()
        {
            foreach (var vrController in _vrControllers)
            {
                ControllersPerDevice.Add(vrController.ControllersDevice, vrController);
            }
        }
    }


    [Serializable]
    public struct VRControllers
    {
        [Tooltip("The VR Device with which this .")]
        public EDevice ControllersDevice;
        public GameObject ControllersMeshPrefab;
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
    }
}