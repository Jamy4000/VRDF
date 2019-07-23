using System.Collections.Generic;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.UI
{
    /// <summary>
    /// Interface that groups all method used to setup the Scrollable UIElements of the Framework
    /// </summary>
    public interface IUISetupScrollable
    {
        /// <summary>
        /// Called in an update method to check if a click is still down
        /// </summary>
        /// <param name="rayHoldingHandle">The hand with with the user is clicking</param>
        /// <param name="clickIsDown">The ClickIsDown BoolVariable Value for the corresponding hand</param>
        void CheckClickStillDown(ref ERayOrigin rayHoldingHandle, bool clickIsDown);

        /// <summary>
        /// Set the UI Scrollable component new value according to the hitPoint, the min and max position and the direction of the element
        /// </summary>
        /// <param name="hitPoint">The hitPoint world position</param>
        /// <param name="minPos">The minimum world position of the scrollable element</param>
        /// <param name="maxPos">The maximum world position of the scrollable element</param>
        /// <returns>The new value of the scrollable element (between 0 and 1)</returns>
        float SetComponentNewValue(Vector3 minPos, Vector3 maxPos, Vector3 hitPoint);

        /// <summary>
        /// Check the value by comparing the distance between the Min and Max Positions and the hitpoint
        /// </summary>
        /// <param name="distanceMinMax">The distance between the minimum and maximum positions of the scrollable</param>
        /// <param name="distanceMinHitPoint">The distance between the min position and the hitpoint</param>
        /// <param name="distanceMaxHitPoint">The distance between the max position and the hitpoint</param>
        /// <returns>The new value of the scrollable element (between 0 and 1)</returns>
        float CheckHitPointInsideComponent(float distanceMinMax, float distanceMinHitPoint, float distanceMaxHitPoint);

        /// <summary>
        /// Call at runtime, set the min and max pos transform by looking in the children of the Handle Rect
        /// </summary>
        /// <param name="minPos">The min pos transform of the Scrollable element</param>
        /// <param name="maxPos">The max pos transform of the Scrollable element</param>
        /// <param name="handleRectParent">The handle rect of the scrollable element</param>
        void SetMinMaxPos(ref Transform minPos, ref Transform maxPos, Transform handleRectParent);

        /// <summary>
        /// Check if the MinPos and MaxPos of the element are correctly instantiated and Check there RectTransform
        /// </summary>
        /// <param name="handleRectParent">The parent of the HandleRect</param>
        /// <param name="direction">The UIDirection of the element</param>
        void CheckMinMaxGameObjects(Transform handleRectParent, EUIDirection direction);

        /// <summary>
        /// If the MinPos or MaxPos Gameobject doesn't exist, instantiate them
        /// </summary>
        /// <param name="name">The name of the object to instantiate</param>
        /// <param name="handleRectParent">The parent of the HandleRect</param>
        /// <return>The RectTransform of the new pos object</return>
        RectTransform InstantiateNewPosObject(string name, Transform handleRectParent);

        /// <summary>
        /// Check for the both min and max pos objects if there rect is set correctly
        /// </summary>
        /// <param name="rect">The rect to check, passed as reference</param>
        /// <param name="anchor">The Vector2 to which we set the pivot, anchor min and anchor max</param>
        void CheckRectTrans(ref RectTransform rect, Vector2 anchor);

        /// <summary>
        /// Check whether the Items present in the scrollview should be active or not
        /// </summary>
        void CheckContentStatus(RectTransform viewport, RectTransform content, bool vertical, bool horizontal);
    }
}