using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;

namespace VRSF.Core.SetupVR
{
    [RequiresEntityConversion]
    public class ControllersAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        [Tooltip("The Device of these controllers.")]
        [SerializeField] public EDevice Device = EDevice.NULL;
        [Tooltip("The controllers themselves.")]
        [SerializeField] public GameObject LeftControllerPrefab;
        [SerializeField] public GameObject RightControllerPrefab;

        #region INTERFACES_OVERRIDES
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(LeftControllerPrefab);
            referencedPrefabs.Add(RightControllerPrefab);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var deviceControllers = new DeviceControllers
            {
                // The referenced prefab will be converted due to DeclareReferencedPrefabs.
                // So here we simply map the game object to an entity reference to that prefab.
                Device = Device,
                LeftControllerEntity = conversionSystem.GetPrimaryEntity(LeftControllerPrefab),
                RightControllerEntity = conversionSystem.GetPrimaryEntity(RightControllerPrefab)
            };

            dstManager.AddComponentData(entity, deviceControllers);
        }
        #endregion INTERFACES_OVERRIDES
    }
}