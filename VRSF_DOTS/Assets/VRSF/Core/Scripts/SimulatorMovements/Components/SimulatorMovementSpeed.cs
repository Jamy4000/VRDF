using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.Simulator
{
    public struct SimulatorMovementSpeed : IComponentData
    {
        /// <summary>
        /// Base Speed of the camera movements.
        /// </summary>
        public float BaseSpeed;

        /// <summary>
        /// Exponential boost factor on translation, controllable by mouse wheel.
        /// </summary>
        public float BaseShiftBoost;

        /// <summary>
        /// Time it takes to interpolate camera position 99% of the way to the target.
        /// </summary>
        public float PositionLerpTime;

        /// <summary>
        /// Used to clamp the BaseShiftBoost
        /// </summary>
        public float LeftShiftBoost
        {
            get => BaseShiftBoost;
            set
            {
                BaseShiftBoost = value;

                if (BaseShiftBoost > 5)
                    BaseShiftBoost = 5;
                else if (BaseShiftBoost < 1)
                    BaseShiftBoost = 1;
            }
        }
    }
}
