using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.Entities;

namespace E7.DataStructure
{
    /// <summary>
    /// Immutable curve completely disconnected from the original <see cref="AnimationCurve"> once instantiated.
    /// Post/pre wrap mode not supported (yet).
    /// Curve weight not supported, until I figured out what is the underlying function.
    /// 
    /// Code taken from https://github.com/5argon/JobAnimationCurve/
    /// </summary>
    public struct JobAnimationCurve : IComponentData, IDisposable
    {
        private struct AnimationData
        {
            public BlobArray<Keyframe> keyframes;
            //This is a hack to help stupid linear interval search, ideally we should find a better way to land on the interval without linear search in the first place.
            public BlobArray<float> soaTimes;
            //More hack to help the stupid linear search, cached index is for my observation that we usually evaluate on the same interval or the next one. 
            //But ideally we have to find better way to land on the correct interval fast without this. (interval tree?)
            public BlobArray<int> cachedIndex;
        }

        private BlobAssetReference<AnimationData> bar;

        // [ReadOnly] [NativeDisableParallelForRestriction] NativeArray<Keyframe> keyframes;
        // [ReadOnly] [NativeDisableParallelForRestriction] NativeArray<float> soaTimes;

        //Not supported yet..
        private WrapMode postWrapMode;
        private WrapMode preWrapMode;
        private InterpolationMode interpolationMode;

        /// <summary>
        /// Used for caching optimization. Each thread would get its own cache area which is selected based on this integer.
        /// </summary>
        [NativeSetThreadIndex] private int threadIndex;

        public int length => bar.Value.keyframes.Length;

        /// <summary>
        /// These are currently useless, just a plan draft..
        /// </summary>
        public enum InterpolationMode
        {
            /// <summary>
            /// This is the same thing as Unity's <see cref="AnimationCurve.Evaluate(float)">. The most accurate but also the most time consuming.
            /// </summary>
            CubicHermiteSpline,

            /// <summary>
            /// Not exactly like <see cref="AnimationCurve.Evaluate(float)">, each hermite basis function are approximated.
            /// </summary>
            ApproximatedCubicHermiteSpline,

            /// <summary>
            /// A horrible optimization that just interpolates as if a straight line is in between every nodes. It is certainly fast though!
            /// 
            /// You are recommended to use <see cref="IncreaseResolution(float)"> first before <see cref="Evaluate(float)"> while in this mode.
            /// So that points that are far apart get more nodes to connect, making linear interpolation more reasonable at the cost of memory.
            /// (If you know you have a highly curved path on far-apart pair of nodes, that will get the most error when linearly interpolated.)
            /// </summary>
            Linear,
        }

        /// <summary>
        /// Replicate all keyframes of <paramref name="animationCurve"> into the struct, 
        /// making this independent from the original reference type curve.
        /// </summary>
        public JobAnimationCurve(AnimationCurve animationCurve, Allocator allocator)
        {
            List<Keyframe> sortedKeyframes = new List<Keyframe>(animationCurve.keys);
            if (sortedKeyframes.Any(x => x.weightedMode != WeightedMode.None))
            {
                throw new NotSupportedException($"Found a keyframe in the curve that has a weighted node. This is not supported until I figured out where to put the weight.");
            }
            sortedKeyframes.Sort(KeyframeTimeSort);

            //Debug.Log(string.Join("\n", sortedKeyframes.Select(x => $"{x.time} {x.value} | {x.inTangent} {x.outTangent} | {x.inWeight} {x.outWeight} {x.weightedMode}")));

            var sortedTimes = sortedKeyframes.Select(x => x.time).ToArray();

            using (var ba = new BlobBuilder())
            {
                ref var root = ref ba.ConstructRoot<AnimationData>();
                int processorCount = SystemInfo.processorCount + 1;
                ba.Allocate(ref root.keyframes, sortedKeyframes.Count);
                ba.Allocate(ref root.soaTimes, sortedKeyframes.Count);
                ba.Allocate(ref root.cachedIndex, processorCount);
                for (int i = 0; i < sortedKeyframes.Count; i++)
                {
                    root.keyframes[i] = sortedKeyframes[i];
                    root.soaTimes[i] = sortedTimes[i];
                }
                for (int i = 0; i < processorCount; i++)
                {
                    root.cachedIndex[i] = 0;
                }

                bar = ba.CreateBlobAssetReference<AnimationData>(allocator);
            }

            postWrapMode = animationCurve.postWrapMode;
            preWrapMode = animationCurve.preWrapMode;
            interpolationMode = InterpolationMode.CubicHermiteSpline;
            threadIndex = 0;

            int KeyframeTimeSort(Keyframe a, Keyframe b) => a.time.CompareTo(b.time);
        }

        /// <summary>
        /// Add more points to the curve while preserving its shape.
        /// </summary>
        /// <param name="minimalInterval">Smallest time interval allowed in the curve. This method will add points in between if it found an interval larger than this.</param>
        public float IncreaseResolution(float minimalInterval)
        {
            throw new NotImplementedException();
        }

        public float Evaluate(float time)
        {
            //TODO : Use interval tree to find neighbouring keyframes of a given time.
            //for (int i = 0; i < bar.Value.soaTimes.Length; i++)
            for (int i = bar.Value.cachedIndex[threadIndex], count = 0;
                count < bar.Value.soaTimes.Length;
                count++, i = (i + 1) % bar.Value.soaTimes.Length)
            {
                //Debug.Log($"Finding at {i} count {count}");
                if (time >= bar.Value.soaTimes[i] && time < bar.Value.soaTimes[i + 1])
                {
                    //cachedIndex[threadIndex] = i;
                    //Debug.Log($"{time} Using keyframe {i} ({soaTimes[i]}) and {i + 1} ({soaTimes[i + 1]})");
                    // System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    // sw.Start();
                    var eval = EvaluateInternal2(
                        time,
                        bar.Value.keyframes[i].time,
                        bar.Value.keyframes[i].value,
                        bar.Value.keyframes[i].outTangent,
                        bar.Value.keyframes[i + 1].time,
                        bar.Value.keyframes[i + 1].value,
                        bar.Value.keyframes[i + 1].inTangent
                    );
                    // sw.Stop();
                    // Debug.Log($"TICKS {sw.ElapsedTicks}");
                    return eval;
                }

            }

            throw new NotSupportedException($"Interval for time {time} not found! Until WrapMode is supported, you must use time that falls inside the curve's data.");
        }

        /// <summary>
        /// This is cubic hermite spline
        /// https://en.wikipedia.org/wiki/Cubic_Hermite_spline
        /// 
        /// i = interval
        /// v = value
        /// t = tangent
        /// 
        /// The function is written unexpanded for easier understanding, 
        /// since I saw Burst is not doing any better with expanded equation (at the current version).
        /// </summary>
        private float EvaluateInternal(float iInterp,
        float iLeft, float vLeft, float tLeft,
        float iRight, float vRight, float tRight)
        {
            float t = math.unlerp(iLeft, iRight, iInterp);

            //TODO: This could be precalculated from when we are creating this struct for each interval? Micro optimization?
            float scale = iRight - iLeft;

            //TODO: Maybe we could create an approximation of each hermite basis that blends into an answer?
            float h00(float x) => (2 * x * x * x) - (3 * x * x) + 1;
            float h10(float x) => (x * x * x) - (2 * x * x) + x;
            float h01(float x) => -(2 * x * x * x) + (3 * x * x);
            float h11(float x) => (x * x * x) - (x * x);

            //TODO: Scaled tangents could also be precalculated. Micro optimization?
            return (h00(t) * vLeft) + (h10(t) * scale * tLeft) + (h01(t) * vRight) + (h11(t) * scale * tRight);
        }

        /// <summary>
        /// Matrix based version in hope that Burst could do better.
        /// </summary>
        private float EvaluateInternal2(float iInterp,
        float iLeft, float vLeft, float tLeft,
        float iRight, float vRight, float tRight)
        {
            float t = math.unlerp(iLeft, iRight, iInterp);
            float scale = iRight - iLeft;

            float4 parameters = new float4(t * t * t, t * t, t, 1);
            float4x4 hermiteBasis = new float4x4(
                2, -2, 1, 1,
                -3, 3, -2, -1,
                0, 0, 1, 0,
                1, 0, 0, 0
            );

            //TODO : Tangent could be prescaled with also precalculated interval size.
            float4 control = new float4(vLeft, vRight, tLeft, tRight) * new float4(1, 1, scale, scale);
            float4 basisWithParams = math.mul(parameters, hermiteBasis);
            float4 hermiteBlend = control * basisWithParams;
            return math.csum(hermiteBlend);
        }

        public void Dispose()
        {
            bar.Dispose();
        }
    }
}