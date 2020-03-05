using UnityEngine;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Instantiate a 3D Model for the specified controller
    /// </summary>
    [RequireComponent(typeof(ControllerMeshesLister))]
    public class ControllerMeshAuthoring : MonoBehaviour
    {
        [Header("The parenting Hand")]
        [Tooltip("To which hand should this controller's mesh be assigned to ? If None, set to the parent of this gameObject.")]
        [SerializeField] private EHand _controllersHand = EHand.NONE;

        [Header("Destroy GameObject when Setup has ended")]
        [Tooltip("Should this gameobject be destroyed when the setup ended, or should only this script be destroyed ?")]
        [SerializeField] private bool _destroyOnSetup = true;

        private void Start()
        {
            OnSetupVRReady.RegisterSetupVRCallback(SetupControllersMesh);
        }

        private void SetupControllersMesh(OnSetupVRReady info)
        {
            OnSetupVRReady.UnregisterSetupVRCallback(SetupControllersMesh);

            var controllersList = GetComponent<ControllerMeshesLister>().ControllersPerDevice;

            if (controllersList.ContainsKey(VRSF_Components.DeviceLoaded))
            {
                // Select the parent depending on the Hand provided in the Inspector
                Transform parent = null;
                switch (_controllersHand)
                {
                    case EHand.LEFT:
                        parent = VRSF_Components.LeftController.transform;
                        break;
                    case EHand.RIGHT:
                        parent = VRSF_Components.RightController.transform;
                        break;
                    default:
                        parent = transform.parent;
                        break;
                }

                // Instantiate the prefab from within the ControllersMeshLister script
                var toInstantiate = controllersList[VRSF_Components.DeviceLoaded];
                var newController = GameObject.Instantiate(toInstantiate.ControllersMeshPrefab, parent).transform;
                newController.localPosition = toInstantiate.LocalPosition;
                newController.localRotation = Quaternion.Euler(toInstantiate.LocalRotation);
            }
            else
            {
                Debug.LogFormat("<b>[VRSF] :</b> No mesh found for the currently loaded VR device " + VRSF_Components.DeviceLoaded + " in the ControllerMeshLister list.", gameObject);
            }

            if (_destroyOnSetup)
                Destroy(gameObject);
            else
                Destroy(this);
        }
    }
}