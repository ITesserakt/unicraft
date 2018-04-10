using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mc2.general {
    public static class GameRegistry {
        
        public static Dictionary<string, GameObject> RegisteredBlocks {
            get { return _registeredBlocks; }
            private set { _registeredBlocks = value; }
        }

        private static uint _bufferedId;
        private static string[] _namesBuffered = new string[0];
        private static Dictionary<string, GameObject> _registeredBlocks = new Dictionary<string, GameObject>();


        public static uint RegId() {
            return _bufferedId++;
        }

        public static void RegSName(string shortName) {
            if (_namesBuffered.Contains(shortName)) {
                throw new ArgumentException("Блок с данным названием уже существует", "shortName");
            }

            Array.Resize(ref _namesBuffered, _namesBuffered.Length + 1);
            _namesBuffered[_namesBuffered.Length - 1] = shortName;

        }

        public static void RegFName(string fullName) {
            if (_namesBuffered.Contains(fullName)) {
                throw new ArgumentException("Блок с данным названием уже существует", "fullName");
            }

            Array.Resize(ref _namesBuffered, _namesBuffered.Length + 1);
            _namesBuffered[_namesBuffered.Length - 1] = fullName;
        }

        public static void RegBlock(GameObject gameObject, Block block) {
            RegisteredBlocks.Add(block.FullName, gameObject);
        }
    }
}