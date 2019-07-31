using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Utils;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// Contains the variables for the PointerClickingSystem. 
    /// WARNING : This needs to be place on the same GameObject as where the VRRaycastAuthoring component is placed.
    /// </summary>
    [RequiresEntityConversion]
    public class PointerClickAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var interactionSet = GetComponent<VRInteractionAuthoring>();
            InteractionSetupHelper.AddInputCaptureComponent(ref dstManager, ref entity, interactionSet);
            InteractionSetupHelper.AddButtonHand(ref dstManager, ref entity, interactionSet.ButtonHand);

            // We add a new pointer click to store
            dstManager.AddComponentData(entity, new PointerClick
            {
                ControllersButton = interactionSet.ButtonToUse,
                HandClicking = interactionSet.ButtonHand,
                CanClick = true
            });

            Destroy(interactionSet);
            Destroy(this);
        }
    }

    public struct PointerClick : IComponentData
    {
        public Inputs.EControllersButton ControllersButton;

        public EHand HandClicking;

        /// <summary>
        /// Whether the user is able to click on stuffs
        /// </summary>
        public bool CanClick;
    }
}