namespace VRSF.Core.Utils
{
    public static class CSharpExtensions
    {
        public static bool IsA<T>(this object obj)
        {
            return obj is T;
        }

        public static bool IsNotA<T>(this object obj)
        {
            return !(obj is T);
        }
    }
}
