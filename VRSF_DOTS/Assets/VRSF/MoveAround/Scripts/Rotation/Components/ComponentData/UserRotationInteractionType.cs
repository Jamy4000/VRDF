using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace VRSF.MoveAround.Rotation
{
    public struct UserRotationInteractionType : IComponentData
    {
        public bool UseClickToRotate;
        public bool UseTouchToRotate;
    }
}
