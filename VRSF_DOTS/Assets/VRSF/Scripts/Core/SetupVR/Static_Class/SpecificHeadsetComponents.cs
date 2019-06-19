using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// Helpers class to add the correct components for each specific headset
    /// </summary>
    public static class SpecificHeadsetComponents
    {
        /// <summary>
        /// Check which device is connected, and set the DeviceToLoad to the right name.
        /// </summary>
        public static void Setup()
        {
            var entityManager = World.Active.EntityManager;
            Entity inputEntity = entityManager.Instantiate(new Entity());

            //switch (VRSF_Components.DeviceLoaded)
            //{
            //    case EDevice.OCULUS_RIFT:
            //        entityManager.AddComponentData(inputEntity, new MoveUp());
            //        entityManager.AddComponentData(inputEntity, new MovingCube());
            //        GameObject.Destroy(setupVR.GetComponent<HtcControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<GoAndGearVRControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
            //        RemoveSimulatorStuffs(setupVR);
            //        CheckControllersReferences(setupVR, setupVR.Rift_Controllers);
            //        break;
            //    case EDevice.OCULUS_RIFT_S:
            //        GameObject.Destroy(setupVR.GetComponent<HtcControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<GoAndGearVRControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
            //        RemoveSimulatorStuffs(setupVR);
            //        CheckControllersReferences(setupVR, setupVR.RiftS_And_Quest_Controllers);
            //        break;
            //    case EDevice.OCULUS_QUEST:
            //        GameObject.Destroy(setupVR.GetComponent<HtcControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<GoAndGearVRControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
            //        RemoveSimulatorStuffs(setupVR);
            //        CheckControllersReferences(setupVR, setupVR.RiftS_And_Quest_Controllers);
            //        break;
            //    case EDevice.OCULUS_GO:
            //        GameObject.Destroy(setupVR.GetComponent<HtcControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<OculusControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
            //        RemoveSimulatorStuffs(setupVR);
            //        CheckControllersReferences(setupVR, setupVR.OculusGO_Controllers);
            //        break;
            //    case EDevice.GEAR_VR:
            //        GameObject.Destroy(setupVR.GetComponent<HtcControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<OculusControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
            //        RemoveSimulatorStuffs(setupVR);
            //        CheckControllersReferences(setupVR, setupVR.GearVR_Controllers);
            //        break;

            //    case EDevice.HTC_VIVE:
            //        GameObject.Destroy(setupVR.GetComponent<GoAndGearVRControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<OculusControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
            //        RemoveSimulatorStuffs(setupVR);
            //        CheckControllersReferences(setupVR, setupVR.Vive_Controllers);
            //        break;
            //    case EDevice.HTC_FOCUS:
            //        GameObject.Destroy(setupVR.GetComponent<GoAndGearVRControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<OculusControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
            //        RemoveSimulatorStuffs(setupVR);
            //        CheckControllersReferences(setupVR, setupVR.Focus_Controllers);
            //        break;

            //    case EDevice.WMR:
            //        GameObject.Destroy(setupVR.GetComponent<GoAndGearVRControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<OculusControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<HtcControllersInputCaptureComponent>());
            //        RemoveSimulatorStuffs(setupVR);
            //        CheckControllersReferences(setupVR, setupVR.WMR_Controllers);
            //        break;

            //    default:
            //        GameObject.Destroy(setupVR.GetComponent<GoAndGearVRControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<OculusControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<HtcControllersInputCaptureComponent>());
            //        GameObject.Destroy(setupVR.GetComponent<WMRControllersInputCaptureComponent>());
            //        CheckControllersReferences(setupVR, setupVR.Simulator_Controllers);
            //        setupVR.StartCoroutine(ResetVRCamera(setupVR));
            //        break;
            //}
        }
        
        private static void RemoveSimulatorStuffs(DeviceToLoadAuthoring setupVR)
        {
            //GameObject.Destroy(setupVR.GetComponent<SimulatorInputCaptureComponent>());
            //GameObject.Destroy(setupVR.CameraRig.GetComponent<SimulatorMovementComponent>());
        }

        /// <summary>
        /// Called to fix a bug with Unity where the TrackedPoseDriver change the transform of
        /// the CameraRig and Controllers even though no device is connected
        /// </summary>
        /// <param name="setupVR"></param>
        /// <returns></returns>
        //private static IEnumerator ResetVRCamera(SetupVRAuthoring setupVR)
        //{
        //    //XRSettings.enabled = false;
        //    //GameObject.Destroy(setupVR.VRCamera.GetComponent<TrackedPoseDriver>());
        //    //GameObject.Destroy(setupVR.LeftController.GetComponent<TrackedPoseDriver>());
        //    //GameObject.Destroy(setupVR.RightController.GetComponent<TrackedPoseDriver>());

        //    //yield return new WaitForEndOfFrame();
        //    //yield return new WaitForEndOfFrame();

        //    //setupVR.VRCamera.transform.localPosition = Vector3.zero;
        //    //setupVR.VRCamera.transform.localRotation = Quaternion.identity;
        //}


        /// <summary>
        /// To setup the controllers reference and instantiate the corresponding models
        /// </summary>
        //private static void CheckControllersReferences(SetupVRAuthoring setupVR, VRController[] Controllers)
        //{
        //    if (setupVR.LeftController == null)
        //    {
        //        Debug.LogError("<b>[VRSF] :</b> No Left Controller was references in SetupVR. Trying to fetch it using tag RESERVED_LeftController.");
        //        setupVR.LeftController = GameObject.FindGameObjectWithTag("RESERVED_LeftController");
        //    }

        //    if (setupVR.RightController == null)
        //    {
        //        Debug.LogError("<b>[VRSF] :</b> No Right Controller was references in SetupVR. Trying to fetch it using tag RESERVED_RightController.");
        //        setupVR.RightController = GameObject.FindGameObjectWithTag("RESERVED_RightController");
        //    }

        //    if (setupVR.LeftController != null)
        //        GameObject.Instantiate(Controllers[0].Hand == EHand.LEFT ? Controllers[0].ControllerPrefab : Controllers[1].ControllerPrefab, setupVR.LeftController.transform);

        //    if (setupVR.RightController != null)
        //        GameObject.Instantiate(Controllers[0].Hand == EHand.RIGHT ? Controllers[0].ControllerPrefab : Controllers[1].ControllerPrefab, setupVR.RightController.transform);
        //}
    }
}