using System;
using System.Collections.Generic;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Controllers
{
    public class ControllerMeshAuthoring : MonoBehaviour
    {
        [Header("The parenting Hand")]
        [Tooltip("To which hand should this controller's mesh be assigned to ?")]
        [SerializeField] private EHand _controllersHand;

        [Header("Destroy GameObject when Setup has ended")]
        [Tooltip("Should this gameobject be destroyed when the setup ended, or should only this script be destroyed ?")]
        [SerializeField] private bool _destroyOnSetup = true;

        [Header("The Controller for this hand")]
        [Tooltip("This component group all controller's mesh for each loadable device. The created Controller will be set as child of the parent from the corresponding hand.")]
        [SerializeField] private VRControllers[] _vrControllers;

        private Dictionary<EDevice, GameObject> _controllersPerDevice = new Dictionary<EDevice, GameObject>();

        private void Awake()
        {
            OnSetupVRReady.Listeners += SetupControllersMesh;
            foreach (var vrController in _vrControllers)
            {
                _controllersPerDevice.Add(vrController.ControllersDevice, vrController.ControllersMeshPrefabs);
            }
        }

        private void OnDestroy()
        {
            OnSetupVRReady.Listeners -= SetupControllersMesh;
        }

        private void SetupControllersMesh(OnSetupVRReady info)
        {
            try
            {
                if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
                {
                    var parent = _controllersHand == EHand.LEFT ? VRSF_Components.LeftController.transform : VRSF_Components.RightController.transform;
                    GameObject.Instantiate(_controllersPerDevice[VRSF_Components.DeviceLoaded], parent);
                }

                if (_destroyOnSetup)
                    Destroy(gameObject);
                else
                    Destroy(this);
            }
            catch
            {
                Debug.LogErrorFormat("[b]VRSF :[\n] Couldn't setup controller, as no value was given for the device {0}. Returning.", VRSF_Components.DeviceLoaded.ToString());
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