using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Core.Interactions
{
    /// <summary>
    /// Contains the variables for the PointerClickingSystem. 
    /// WARNING : This needs to be place on the same GameObject as where the VRRaycastAuthoring component is placed.
    /// </summary>
    [RequiresEntityConversion]
    public class PointerClickAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("The Hand attached to this click interaction")]
        [Tooltip("We always use the trigger for clicking on stuff. If you wanna modify that, check the TriggerInputCapture component in script by the one you want to use.")]
        public EHand HandClicking = EHand.NONE;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // We add a new trigger input capture as we click using this trigger
            dstManager.AddComponentData(entity, new TriggerInputCapture
            {
                Hand = HandClicking
            });

            // We add a new pointer click to store
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
        /// Whether the user is able to click on stuffs
        /// </summary>
        public bool CanClick;

        /// <summary>
        /// Whether the click event was already fired. Avoid the repeating of the event
        /// </summary>
        public bool ClickEventWasFired;
    }
}