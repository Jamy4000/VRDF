﻿namespace VRSF.Core.Utils
{
    public struct STuples<T1, T2>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;

        public STuples(T1 item1, T2 item2) { Item1 = item1; Item2 = item2; }
    }
}