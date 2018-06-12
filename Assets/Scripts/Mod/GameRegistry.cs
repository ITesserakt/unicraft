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

        internal static string RegSName(string shortName) {
            if (NamesBuffered.Contains(shortName)) {
                throw new ArgumentException("Блок с данным названием уже существует", nameof(shortName));
            }

            NamesBuffered.Add(shortName);
            return shortName;
        }

        internal static string RegFName(string fullName) {
            if (NamesBuffered.Contains(fullName)) {
                throw new ArgumentException("Блок с данным названием уже существует", nameof(fullName));
            }

            NamesBuffered.Add(fullName);
            return fullName;
        }

        public static void RegWorldGen(IItem block, int chance, int latitude) {
            BlockSpawnChance.Add(new GenerationProperties(block, chance, latitude));
        }

        internal static void RegBlock(GameObject gameObject, IItem block) {
            RegisteredBlocks.Add(block.FullName, gameObject);
        }
    }

    internal class GenerationProperties {
        internal IItem Item { get; }
        internal int ChanceToSpawn { get; }
        internal int Latitude { get; }

        public GenerationProperties(IItem item, int chanceToSpawn, int latitude) {
            Item = item;
            ChanceToSpawn = chanceToSpawn;
            Latitude = latitude;
        }
    }
}