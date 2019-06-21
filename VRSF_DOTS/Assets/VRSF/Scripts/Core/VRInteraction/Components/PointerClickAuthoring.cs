using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Interactions
{
    /// <summary>
    /// Contains the variables for the OnColliderClickSystems
    /// </summary>
    public class PointerClickAuthoring : MonoBehaviour
    {
        public EHand HandClicking = EHand.NONE;

        private void Awake()
        {
            // We get the EntityManager
            EntityManager entityManager = World.Active.EntityManager;

            EntityArchetype archetype = entityManager.CreateArchetype
            (
                typeof(PointerClick)
            );

            // We create the entity based on the archetype we juste created
            Entity raycastEntity = entityManager.CreateEntity(archetype);

            // We add the VRRaycastParameters as a struct to the newly created entity
            entityManager.SetComponentData(raycastEntity, new PointerClick
            {
                HandClicking = HandClicking
            });

#if UNITY_EDITOR
            // Set the name of the entity in Editor Mode for the Entity Debugger Window
            entityManager.SetName(raycastEntity, string.Format("Pointer Click " + HandClicking.ToString(), raycastEntity.Index));
#endif

            // We destroy this component as we don't need it anymore
            Destroy(this);
        }
    }

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