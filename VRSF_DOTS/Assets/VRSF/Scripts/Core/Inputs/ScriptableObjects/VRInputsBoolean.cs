using ScriptableFramework.RuntimeSet;
using ScriptableFramework.Variables;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    [CreateAssetMenu(menuName = "RuntimeSet/Dictionnary/VRInputsBool")]
    public class VRInputsBoolean : ScriptableDictionnary<string, BoolVariable>
    {
    }
}