using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.UI
{
    /// <summary>
    /// Class that implement the IUISetupScrollable, used in the Scrollable UI Elements.
    /// </summary>
	public class VRUIScrollableSetup
    {
        public VRUIScrollableSetup(EUIDirection dir, float minVal = 0.0f, float maxVal = 1.0f, bool wholeNum = false)
        {
            _direction = dir;
            _minValue = minVal;
            _maxValue = maxVal;
            _wholeNumbers = wholeNum;
        }


        #region PRIVATE_VARIABLES
        private readonly EUIDirection _direction;
        private readonly float _minValue = 0.0f;
        private readonly float _maxValue = 1.0f;
        private readonly bool _wholeNumbers = false;
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Called in an update method to check if a click is still down
        /// </summary>
        /// <param name="handHoldingHandle">The hand with with the user is clicking</param>
        /// <param name="clickIsDown">The ClickIsDown BoolVariable Value for the corresponding hand</param>
        public void CheckClickStillDown(ref ERayOrigin handHoldingHandle, bool clickIsDown)
        {
            if (!clickIsDown)
                handHoldingHandle = ERayOrigin.NONE;
        }


        /// <summary>
        /// Set the UI Scrollable component new value according to the hitPoint, the min and max position and the direction of the element
        /// </summary>
        /// <param name="minPos">The minimum world position of the scrollable element</param>
        /// <param name="maxPos">The maximum world position of the scrollable element</param>
        /// <param name="hitPoint">The hitPoint world position</param>
        /// <returns>The new value of the scrollable element (between 0 and 1)</returns>
        public float SetComponentNewValue(Vector3 minPos, Vector3 maxPos, Vector3 hitPoint)
        {
            float distanceMinMax = Vector3.Distance(minPos, maxPos);
            float distanceMaxHitPoint;
            float distanceMinHitPoint;

            // IF direction is reverse, we reverse the value. doesn't work if you change the direction while in play mode.
            if (_direction == EUIDirection.LeftToRight || _direction == EUIDirection.BottomToTop)
            {
                distanceMaxHitPoint = Vector3.Distance(minPos, hitPoint);
                distanceMinHitPoint = Vector3.Distance(maxPos, hitPoint);
            }
            else
            {
                distanceMaxHitPoint = Vector3.Distance(maxPos, hitPoint);
                distanceMinHitPoint = Vector3.Distance(minPos, hitPoint);
            }

            float toReturn = CheckHitPointInsideComponent(distanceMinMax, distanceMinHitPoint, distanceMaxHitPoint);
            return _wholeNumbers ? (int)toReturn : toReturn;
        }


        /// <summary>
        /// Check the value by comparing the distance between the Min and Max Positions and the hitpoint
        /// </summary>
        /// <param name="distanceMinMax">The distance between the minimum and maximum positions of the scrollable</param>
        /// <param name="distanceMinHitPoint">The distance between the min position and the hitpoint</param>
        /// <param name="distanceMaxHitPoint">The distance between the max position and the hitpoint</param>
        /// <returns>The new value of the scrollable element (between 0 and 1)</returns>
        public float CheckHitPointInsideComponent(float distanceMinMax, float distanceMinHitPoint, float distanceMaxHitPoint)
        {
            if (distanceMaxHitPoint > distanceMinMax)
                return _minValue;
            else if (distanceMinHitPoint > distanceMinMax)
                return _maxValue;
            else
                return (distanceMinHitPoint / distanceMinMax) * _maxValue;
        }


        /// <summary>
        /// Check if the MinPos and MaxPos of the element are correctly instantiated and Check there RectTransform
        /// </summary>
        /// <param name="handleRectParent">The parent of the HandleRect</param>
        /// <param name="direction">The UIDirection of the element</param>
        public void CheckMinMaxGameObjects(Transform handleRectParent, EUIDirection direction, ref Transform minPosRect, ref Transform maxPosRect)
        {
            minPosRect = handleRectParent.Find("MinPos") == null ? InstantiateNewPosObject("MinPos", handleRectParent) : handleRectParent.Find("MinPos").GetComponent<RectTransform>();
            maxPosRect = handleRectParent.Find("MaxPos") == null ? InstantiateNewPosObject("MaxPos", handleRectParent) : handleRectParent.Find("MaxPos").GetComponent<RectTransform>();

            if (direction == EUIDirection.BottomToTop || direction == EUIDirection.TopToBottom)
            {
                CheckRectTrans(ref minPosRect, new Vector2(0.5f, 1.0f));
                CheckRectTrans(ref maxPosRect, new Vector2(0.5f, 0.0f));
            }
            else
            {
                CheckRectTrans(ref minPosRect, new Vector2(1.0f, 0.5f));
                CheckRectTrans(ref maxPosRect, new Vector2(0.0f, 0.5f));
            }
        }

        /// <summary>
        /// If the MinPos or MaxPos Gameobject doesn't exist, instantiate them
        /// </summary>
        /// <param name="name">The name of the object to instantiate</param>
        /// <param name="handleRectParent">The parent of the HandleRect</param>
        /// <return>The RectTransform of the new pos object</return>
        public RectTransform InstantiateNewPosObject(string name, Transform handleRectParent)
        {
            GameObject newPos = new GameObject(name);
            newPos.transform.SetParent(handleRectParent);
            return newPos.AddComponent<RectTransform>();
        }

        /// <summary>
        /// Check for the both min and max pos objects if there rect is set correctly
        /// </summary>
        /// <param name="rect">The rect to check, passed as reference</param>
        /// <param name="anchor">The Vector2 to which we set the pivot, anchor min and anchor max</param>
        public void CheckRectTrans(ref Transform trans, Vector2 anchor)
        {
            var rect = trans as RectTransform;
            rect.anchorMax = anchor;
            rect.anchorMin = anchor;
            rect.pivot = anchor;
            rect.localScale = Vector3.one;
            rect.sizeDelta = Vector2.zero;
            rect.localRotation = Quaternion.Euler(Vector3.zero);
            rect.anchoredPosition3D = Vector3.zero;
        }

        /// <summary>
        /// Check whether the Items present in the scrollview should be active or not
        /// </summary>
        /// <param name="viewport">The viewport for our scrollable</param>
        /// <param name="content">the content of our scrollable</param>
        public void CheckContentStatus(RectTransform viewport, RectTransform content)
        {
            foreach (var collider in content.GetComponentsInChildren<Collider>())
            {
                try
                {
                    collider.enabled = RectTransformUtility.RectangleContainsScreenPoint(viewport, new Vector2(collider.transform.position.x, collider.transform.position.y));
                }
                catch (System.Exception e)
                {
                    Debug.Log("<b>[VRSF] :</b> Cannot check if collider in scrollview is visible. Setting the colldier at true. Error is as follow :\n" + e.ToString());
                    collider.enabled = true;
                }
            }
        }
        #endregion
    }
}