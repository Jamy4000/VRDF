using UnityEngine;

namespace ScriptableFramework.Variables
{
    public class VariableBase<t> : ScriptableObject
    {
        [Multiline]
        public string DeveloperDescription = "";
        public t Value;

        public void SetValue(t value)
        {
            Value = value;
        }

        public void SetValue(VariableBase<t> value)
        {
            Value = value.Value;
        }
    }
}