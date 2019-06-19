using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.SetupVR
{
    [RequiresEntityConversion]
    public class VRObjectsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("The references to the VR GameObjects.")]
        public GameObject CameraRig;
        public Transform FloorOffset; 
        public GameObject LeftController;
        public GameObject RightController;
        public GameObject VRCamera;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            SetupVRInScene();
        }


        /// <summary>
        /// Method called on Awake and in Update, if the setup is not finished, 
        /// to load the VR SDK Prefab and set its parameters.
        /// </summary>
        void SetupVRInScene()
        {
            // We seperate the camera rig from SetupVR
            CameraRig.transform.SetParent(null);

            if (FloorOffset == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No Floor Offset was references in SetupVR. Trying to fetch it using the name Floor_Offset.");
                FloorOffset = GameObject.Find("Floor_Offset").transform;
            }

            VRSF_Components.FloorOffset = FloorOffset;

            if (CameraRig == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No CameraRig was references in SetupVR. Trying to fetch it using tag RESERVED_CameraRig.");
                CameraRig = GameObject.FindGameObjectWithTag("RESERVED_CameraRig");
            }

            VRSF_Components.CameraRig = CameraRig;

            // We set the references to the VRCamera
            if (VRCamera == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No VRCamera was references in SetupVR. Trying to fetch it using tag MainCamera.");
                VRCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            VRSF_Components.VRCamera = VRCamera;

            VRSF_Components.LeftController = LeftController;
            VRSF_Components.RightController = RightController;

            VRSF_Components.SetupVRIsReady = true;
            new OnSetupVRReady();
        }
    }
}