using System;
using UnityEngine;

namespace mc2.general
{
    public sealed class GameEvents
    {
        public const uint ManagersStarted = 101;
        public const uint ManagersInProgress = 102;
        public const uint RightCl = 203;
        public const uint LeftCl = 204;
        public const uint MidCl = 205;
		public const uint BlockUpdate = 306;
    }

    public sealed class RightClickArgs {
        public RaycastHit Hit { get; private set; }
        public Transform Block { get; private set; }

        public RightClickArgs(RaycastHit hit, Transform block) {
            Hit = hit;
            Block = block;
        }
    }
}