namespace VRSF.Core.SetupVR
{
    public class InputAxis
    {
        public string Name;
        public string DescriptiveName;
        public string DescriptiveNegativeName;
        public string NegativeButton;
        public string PositiveButton;
        public string AltNegativeButton;
        public string AltPositiveButton;

        public float Gravity;
        public float Dead;
        public float Sensitivity;

        public bool Snap = false;
        public bool Invert = false;

        public AxisType Type;

        public int Axis;
        public int JoyNum;
    }
}