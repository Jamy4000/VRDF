using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Controllers
{
    [RequireComponent(typeof(ControllerMeshListing))]
    public class ControllerMeshAuthoring : MonoBehaviour
    {
        [Header("The parenting Hand")]
        [Tooltip("To which hand should this controller's mesh be assigned to ?")]
        [SerializeField] private EHand _controllersHand;

        [Header("Destroy GameObject when Setup has ended")]
        [Tooltip("Should this gameobject be destroyed when the setup ended, or should only this script be destroyed ?")]
        [SerializeField] private bool _destroyOnSetup = true;

        private void Start()
        {
            OnSetupVRReady.RegisterSetupVRResponse(SetupControllersMesh);
        }

        private void OnDestroy()
        {
            if (OnSetupVRReady.IsMethodAlreadyRegistered(SetupControllersMesh))
                OnSetupVRReady.Listeners -= SetupControllersMesh;
        }

        private void SetupControllersMesh(OnSetupVRReady info)
        {
            try
            {
                if (VRSF_Components.DeviceLoaded != EDevice.SIMULATOR)
                {
                    var controllersList = GetComponent<ControllerMeshListing>().ControllersPerDevice;
                    var parent = _controllersHand == EHand.LEFT ? VRSF_Components.LeftController.transform : VRSF_Components.RightController.transform;
                    GameObject.Instantiate(controllersList[VRSF_Components.DeviceLoaded], parent);
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
}