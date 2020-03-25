using UnityEngine;

namespace VRDF.Core.SetupVR
{
    /// <summary>
    /// Allow us to have direct references to the VR Objects in the scene (CameraRig, Camera, Floor Offset and Controllers)
    /// </summary>
    public class VRObjectsAuthoring : MonoBehaviour
    {
        [Header("The references to the VR GameObjects.")]
        [SerializeField] private GameObject _cameraRig;
        [SerializeField] private GameObject _floorOffset;
        [SerializeField] private GameObject _leftController;
        [SerializeField] private GameObject _rightController;
        [SerializeField] private GameObject _vrCamera;

        public void Awake()
        {
            SetVRDFComponents();

            // We seperate the camera rig from SetupVR
            _cameraRig.transform.SetParent(null);
        }

        /// <summary>
        /// Check for each GameObject that the field set in editor is correct, and if not, look for those object using a tag
        /// </summary>
        private void SetVRDFComponents()
        {
            VRDF_Components.CameraRig = CheckProvidedGameObject(_cameraRig, "RESERVED_CameraRig");
            VRDF_Components.FloorOffset = CheckProvidedGameObject(_floorOffset, "Floor_Offset");
            VRDF_Components.VRCamera = CheckProvidedGameObject(_vrCamera, "MainCamera");
            VRDF_Components.LeftController = CheckProvidedGameObject(_leftController, "RESERVED_LeftController");
            VRDF_Components.RightController = CheckProvidedGameObject(_rightController, "RESERVED_RightController");

            VRDF_Components.SetVRCameraPosition(_cameraRig.transform.position);


            GameObject CheckProvidedGameObject(GameObject toCheck, string tag)
            {
                // We set the references to the Right COntroller
                if (toCheck == null)
                {
                    Debug.LogErrorFormat("<b>[VRDF] :</b> No {0} was references in VRObjectsAuthoring. Trying to fetch it using tag {1}.", toCheck.transform.name, tag, gameObject);
                    toCheck = GameObject.FindGameObjectWithTag(tag);
                }

                return toCheck;
            }
        }
    }
}