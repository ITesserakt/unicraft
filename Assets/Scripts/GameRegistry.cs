using System;
using System.Linq;
using UnityEngine;

namespace mc2.general {
    public static class GameRegistry {

        private static uint id;
        private static string[] _namesBuffered = new string[0];

        private const string Exception = "Блок с данным {0} уже существует; последний {0} - ";

        public static uint RegId() {
            return id++;
        }

        public static void RegSName(string shortName) {
            if (_namesBuffered.Contains(shortName)) {
                throw new ArgumentException(
                    string.Format(Exception, "короткое название") + _namesBuffered[_namesBuffered.Length - 1],
                    "shortName");
            }

            Array.Resize(ref _namesBuffered, _namesBuffered.Length + 1);
            _namesBuffered[_namesBuffered.Length - 1] = shortName;

        }

        public static void RegFName(string fullName) {
            if (_namesBuffered.Contains(fullName)) {
                throw new ArgumentException(
                    string.Format(Exception, "полное название") + _namesBuffered[_namesBuffered.Length - 1],
                    "fullName");
            }

            Array.Resize(ref _namesBuffered, _namesBuffered.Length + 1);
            _namesBuffered[_namesBuffered.Length - 1] = fullName;
        }
    }
}