using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Core.Utils
{
    public static class LayerMaskExtension
    {
        public static List<GameObject> GetObjectsInLayer(GameObject root, int layer)
        {
            var ret = new List<GameObject>();
            foreach (Transform t in root.GetComponentsInChildren(typeof(Transform), true))
            {
                if (t.gameObject.layer == layer)
                    ret.Add(t.gameObject);
            }
            return ret;
        }

        public static List<GameObject> GetObjectsInLayer(GameObject root, string layerName)
        {
            var ret = new List<GameObject>();
            foreach (Transform t in root.GetComponentsInChildren(typeof(Transform), true))
            {
                if (LayerMask.LayerToName(t.gameObject.layer) == layerName)
                    ret.Add(t.gameObject);
            }
            return ret;
        }

        public static LayerMask NamesToMask(params string[] layerNames)
        {
            LayerMask ret = (LayerMask)0;
            foreach (var name in layerNames)
            {
                ret |= (1 << LayerMask.NameToLayer(name));
            }
            return ret;
        }


        /// <summary>
        /// Extension method to check if a layer is in a layermask
        /// </summary>
        /// <param name="mask">The layerMask to check</param>
        /// <param name="layer">The layer to check</param>
        /// <returns>true if the layer is in the layerMask</returns>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        /// <summary>
        /// Add a Layer to a LayerMask
        /// </summary>
        /// <param name="mask">The layerMask to check</param>
        /// <param name="layerName">The layer to add</param>
        /// <returns>true if the layer is in the layerMask</returns>
        public static LayerMask AddToMask(this LayerMask original, string layerName)
        {
            return original | (1 << LayerMask.NameToLayer(layerName));
        }

        /// <summary>
        /// Add a Layer to a LayerMask
        /// </summary>
        /// <param name="mask">The layerMask to check</param>
        /// <param name="layer">The layer to add</param>
        /// <returns>true if the layer is in the layerMask</returns>
        public static LayerMask AddToMask(this LayerMask original, int layer)
        {
            return original | (1 << layer);
        }


        /// <summary>
        /// Remove a Layer from a LayerMask
        /// </summary>
        /// <param name="mask">The layerMask to check</param>
        /// <param name="layerName">The layer to remove</param>
        /// <returns>true if the layer is in the layerMask</returns>
        public static LayerMask RemoveFromMask(this LayerMask original, string layerName)
        {
            return original & ~(1 << LayerMask.NameToLayer(layerName));
        }


        /// <summary>
        /// Remove a Layer from a LayerMask
        /// </summary>
        /// <param name="mask">The layerMask to check</param>
        /// <param name="layer">The layer to remove</param>
        /// <returns>true if the layer is in the layerMask</returns>
        public static LayerMask RemoveFromMask(this LayerMask original, int layer)
        {
            return original & ~(1 << layer);
        }


        public static LayerMask Inverse(this LayerMask original)
        {
            return ~original;
        }

        public static string[] MaskToNames(this LayerMask original)
        {
            var output = new List<string>();

            for (int i = 0; i < 32; ++i)
            {
                int shifted = 1 << i;
                if ((original & shifted) == shifted)
                {
                    string layerName = LayerMask.LayerToName(i);
                    if (!string.IsNullOrEmpty(layerName))
                    {
                        output.Add(layerName);
                    }
                }
            }
            return output.ToArray();
        }


        /// <summary>
        /// Allow us to get the names of all the layers
        /// </summary>
        /// <returns>The list of layers names</returns>
        public static string[] GetLayerMaskList()
        {
            List<string> layerNames = new List<string>();

            for (int i = 0; i <= 31; i++) // Unity supports 31 layers
            {
                layerNames.Add(LayerMask.LayerToName(i));
            }

            return layerNames.ToArray();
        }
    }
}