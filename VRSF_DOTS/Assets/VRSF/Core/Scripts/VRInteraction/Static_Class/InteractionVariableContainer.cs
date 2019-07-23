using UnityEngine;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// TODO Refactor, static classes shouldn't be used in Jobs
    /// </summary>
	public static class InteractionVariableContainer
    {
        /// <summary>
        /// Bool to verify if something is Hit
        /// </summary>
        public static bool IsClickingSomethingRight;
        public static bool IsClickingSomethingLeft;
        public static bool IsClickingSomethingGaze;
        
        /// <summary>
        /// Bool to verify if something is being hovered
        /// </summary>
        public static bool IsOverSomethingRight;
        public static bool IsOverSomethingLeft;
        public static bool IsOverSomethingGaze;

        /// <summary>
        /// The previous Transform hit for the Controllers and Gaze.
        /// </summary>
        public static Transform PreviousRightHit;
        public static Transform PreviousLeftHit;
        public static Transform PreviousGazeHit;
    }
}