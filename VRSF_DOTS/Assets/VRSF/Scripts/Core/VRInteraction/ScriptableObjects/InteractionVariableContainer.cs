
using ScriptableFramework.Utils;
using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Interactions
{
	public class InteractionVariableContainer : ScriptableSingleton<InteractionVariableContainer>
    {
        #region PUBLIC_VARIABLES
        [Multiline(10)]
        public string DeveloperDescription = "";

        [Header("The RayVariable for the Controllers and Gaze.")]
        public RayVariable RightRay;
        public RayVariable LeftRay;
        public RayVariable GazeRay;

        [Header("The RaycastHitVariable for the Controllers and Gaze.")]
        public RaycastHitVariable RightHit;
        public RaycastHitVariable LeftHit;
        public RaycastHitVariable GazeHit;

        [Header("BoolVariable to verify if something is Hit")]
        public BoolVariable HasClickSomethingRight;
        public BoolVariable HasClickSomethingLeft;
        public BoolVariable HasClickSomethingGaze;

        [Header("BoolVariable to verify if something is Hit")]
        public BoolVariable IsOverSomethingRight;
        public BoolVariable IsOverSomethingLeft;
        public BoolVariable IsOverSomethingGaze;


        [Header("The previous RaycastHitVariable for the Controllers and Gaze.")]
        [HideInInspector] public Transform PreviousRightHit;
        [HideInInspector] public Transform PreviousLeftHit;
        [HideInInspector] public Transform PreviousGazeHit;
        #endregion
    }
}