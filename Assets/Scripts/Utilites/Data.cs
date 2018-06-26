using System;
using System.Collections.Generic;
using UnityEngine;

namespace mc2.utils {
    public class Data : MonoBehaviour {
        public List<Mesh> Meshes;
        public static string AppFolder => SetAppFolder();
        public static string PluginsFolder => AppFolder + "/Plugins";
        public static GameObject Player { get; private set; }

        private void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        private static string SetAppFolder()
        {
#if UNITY_STANDALONE_LINUX
            return @"/.config/MC2";
#endif
#if UNITY_STANDALONE_WIN
            return @"C://Games/MC2";
#endif
#if UNITY_METRO
            return "";
#endif
        }
    }
}