#if UNITY_EDITOR
namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Used in the ViveNavMeshEditor script.
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public struct Edge
    {
        public int Min
        {
            get; private set;
        }
        public int Max
        {
            get; private set;
        }

        public Edge(int p1, int p2)
        {
            // This is done so that if p1 and p2 are switched, the edge has the same hash
            Min = p1 < p2 ? p1 : p2;
            Max = p1 >= p2 ? p1 : p2;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge))
                return false;

            Edge e = (Edge)obj;
            return e.Min == Min && e.Max == Max;
        }

        public override int GetHashCode()
        {
            // Small note: this breaks when you have more than 65535 vertices.
            return (Min << 16) + Max;
        }
    }
}
#endif