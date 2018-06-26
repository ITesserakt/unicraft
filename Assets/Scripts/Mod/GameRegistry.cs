using System;
using System.Collections.Generic;
using UnityEngine;

namespace mc2.mod
{
    public static class GameRegistry {
        
        internal static Dictionary<string, GameObject> RegisteredBlocks { get; }

        internal static List<GenerationProperties> BlockSpawnChance { get; }

        private static int _bufferedId;
        private static readonly List<string> NamesBuffered = new List<string>();

        static GameRegistry() {
            RegisteredBlocks = new Dictionary<string, GameObject>();
            BlockSpawnChance = new List<GenerationProperties>();
        }

        internal static int RegId() {
            return _bufferedId++;
        }

        internal static string RegName(string name) {
            if (NamesBuffered.Contains(name)) {
                throw new ArgumentException($"Блок с данным названием ({name}) уже существует", nameof(name));
            }

            NamesBuffered.Add(name);
            return name;
        }

        public static void RegWorldGen(IItem block, int chance, int latitude, bool invertLatitude = false) {
            BlockSpawnChance.Add(new GenerationProperties(block, chance, latitude, invertLatitude));
        }

        internal static void RegBlock(GameObject gameObject, IItem block) {
            RegisteredBlocks.Add(block.FullName, gameObject);
        }
    }

    internal class GenerationProperties {
        internal IItem Item { get; }
        internal int ChanceToSpawn { get; }
        internal int Latitude { get; }
        internal bool Invert { get; }

        public GenerationProperties(IItem item, int chanceToSpawn, int latitude, bool invertLat) {
            Item = item;
            ChanceToSpawn = chanceToSpawn;
            Latitude = latitude;
            Invert = invertLat;
        }
    }
}