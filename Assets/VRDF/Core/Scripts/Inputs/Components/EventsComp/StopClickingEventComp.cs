﻿using Unity.Entities;

namespace VRDF.Core.Inputs
{
    public struct StopClickingEventComp : IComponentData
    {
        public EControllersButton ButtonInteracting;

        public bool HasWaitedOneFrameBeforeRemoval;
    }
}