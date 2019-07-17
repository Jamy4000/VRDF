using UnityEngine;

namespace VRSF.Core.Utils
{
    /// <summary>
    /// Allow to make a research from a Transform into the GrandChildren.
    /// Usefull as the Transform.Find method only work for the children, and not the GrandChildren.
    /// </summary>
    public static class TransformDeepChildExtension
    {
        /// <summary>
        /// Breadth-first search with string as parameter
        /// </summary>
        /// <param name="aParent">The parent of the transform to find</param>
        /// <param name="aDeepChild">The name of the Transform to find</param>
        /// <returns></returns>
        public static Transform FindDeepChild(this Transform aParent, string aDeepChild, bool nameShouldExactlyMatch = false)
        {
            var result = aParent.Find(aDeepChild);

            if (result != null)
                return result;

            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(aDeepChild);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}