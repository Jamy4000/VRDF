using Photon.Pun;
using UnityEngine;
using VRDF.Core.Controllers;
using VRDF.Core.SetupVR;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Used to display a different body for the remote and local users
    /// Need to be placed under the Controllers Transform object from the PlayerPrefab
    /// </summary>
    [RequireComponent(typeof(PhotonView), typeof(ControllerMeshesLister))]
    public class VRDFRemoteControllersInstantiater : MonoBehaviourPun
    {
        private void Start()
        {
            // For Local Player, place a Basic ControllerMeshAuthoring under your SetupVR prefab.
            if (photonView.IsMine)
                Destroy(gameObject);
            else
                InstantiateRemoteControllers();
        }

        /// <summary>
        /// Callback for whenever one of the player had sent its device info
        /// Generate the corresponding mesh for the used controllers
        /// </summary>
        /// <param name="info"></param>
        private void InstantiateRemoteControllers()
        {
            EDevice deviceUsed = (EDevice)photonView.Owner.CustomProperties[VRDFPlayer.DEVICE_USED];
            var controllersList = GetComponent<ControllerMeshesLister>().ControllersPerDevice;

            if (controllersList.ContainsKey(deviceUsed))
            {
                // Instantiate the prefab from within the ControllersMeshLister script
                var toInstantiate = controllersList[deviceUsed];
                var controllerInstance = GameObject.Instantiate(toInstantiate.ControllersMeshPrefab, transform.parent);
                controllerInstance.transform.localPosition = toInstantiate.LocalPosition;
                controllerInstance.transform.localRotation = Quaternion.Euler(toInstantiate.LocalRotation);
                controllerInstance.tag = "Untagged";

                for (int i = 0; i < controllerInstance.transform.childCount; i++)
                    controllerInstance.transform.GetChild(i).tag = "Untagged";
            }
            else
            {
                Debug.LogFormat("<b>[VRDF] :</b> No mesh found for the Remote Player " + photonView.Owner.NickName + " with VR device " + deviceUsed + " in the ControllerMeshLister list.", gameObject);
            }
        }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(VRDFRemoteControllersInstantiater))]
    public class VRDFRemoteControllersInstantiaterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UnityEditor.EditorGUILayout.HelpBox("For Local Player, please use a basic ControllerMeshAuthoring component by placing it under your SetupVR prefab.\n" +
                "A prefab is available by Right Clicking in the Hierarchy > VRDF > Controllers > Left/Right Controller", UnityEditor.MessageType.Info);
        }
    }
#endif
}