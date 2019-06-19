using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.SetupVR
{ 
    [RequiresEntityConversion]
    public class DeviceToLoadAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("VR Device Parameters.")]
        [Tooltip("The Device you want to load.")]
        [SerializeField]
        public EDevice Device;

        [Tooltip("If false, the device to load will be set with your Editor choice or with a potential starting screen choice.")]
        [SerializeField]
        public bool CheckDeviceAtRuntime;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var deviceToLoad = new DeviceToLoad
            {
                // The referenced prefab will be converted due to DeclareReferencedPrefabs.
                // So here we simply map the game object to an entity reference to that prefab.
                Device = Device,
                ShouldCheckConnectedDevice = CheckDeviceAtRuntime
            };

            dstManager.AddComponentData(entity, deviceToLoad);
        }
    }
}