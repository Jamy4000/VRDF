using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Interactions
{
    /// <summary>
    /// Contains the variables for the OnColliderClickSystems
    /// </summary>
    [RequiresEntityConversion]
    public class PointerClickAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public EHand HandClicking = EHand.NONE;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new PointerClick
            {
                HandClicking = HandClicking
            });
        }
    }

    [RequireComponentTag(typeof(Raycast.VRRaycastAuthoring))]
    public struct PointerClick : IComponentData
    {
        public EHand HandClicking;

        /// <summary>
        /// Whether the user is able to click on stuffs with the left trigger
        /// </summary>
        public static bool LeftTriggerCanClick = true;

        /// <summary>
        /// Whether the user is able to click on stuffs with the right trigger
        /// </summary>
        public static bool RightTriggerCanClick = true;
    }
}