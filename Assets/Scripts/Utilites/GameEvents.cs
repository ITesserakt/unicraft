using System;
using UnityEngine;

namespace mc2.utils
{
    public static class GameEvents
    {
        public const byte   ManagersStarted     = 101;
        public const byte   ManagersInProgress  = 102;
        public const byte   ChangeScene         = 103;
        
        public const byte   RightCl             = 201;
        public const byte   LeftCl              = 202;
        public const byte   MidCl               = 203;
        
		public const ushort BlockUpdate         = 301;
    }
}