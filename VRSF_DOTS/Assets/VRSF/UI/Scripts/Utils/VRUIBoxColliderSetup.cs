using UnityEngine;

namespace VRSF.UI
{
    /// <summary>
    /// This class is called in every UI Elements to setup the gameEvents and there Listeners and add the response to them.
    /// </summary>
	public static class VRUIBoxColliderSetup
    {
        #region PUBLIC_METHODS
        /// <summary>
        /// Set the size of the BoxCollider to fit the UI Element
        /// </summary>
        /// <param name="box">The box collider to set</param>
        /// <param name="rect">the rect Transform of the object attached to the BoxCollider</param>
        /// <return>The Box Colldier updated</return>
        public static BoxCollider CheckBoxColliderSize(BoxCollider box, RectTransform rect)
        {
            // Set box size
            box.size = new Vector3(rect.rect.width * rect.localScale.x, rect.rect.height * rect.localScale.x, 0.001f);

            // Set box center
            float x = GetBoxCenter(rect.pivot.x, box.size.x);
            float y = GetBoxCenter(rect.pivot.y, box.size.y);
            box.center = new Vector3(x, y, box.center.z);

            return box;
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check one of the pivot axis of the UIElement to return one of the axis of the BoxCollider center
        /// </summary>
        /// <param name="pivotAxis">The x or y value of the UIElement's pivot</param>
        /// <param name="boxSizeAxis">The x or y value of the BoxCollider size</param>
        /// <returns>One of the axis of the BoxCollider center</returns>
        private static float GetBoxCenter(float pivotAxis, float boxSizeAxis)
        {
            if (pivotAxis > 0.5f)
            {
                return -boxSizeAxis / 2;
            }
            else if (pivotAxis < 0.5f)
            {
                return boxSizeAxis / 2;
            }
            else    // pivotAxis == 0.0f
            {
                return 0.0f;
            }
        }
        #endregion
    }
}