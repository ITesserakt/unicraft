using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using mc2.mod;
using mc2.utils;
using UniRx;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;

namespace mc2.managers {
    [DontLoadOnStatup]
    public class WorldGenerator : GameManager {
        public const byte WidthForChunk = 16;

        [Range(4, 1024)] [SerializeField] private int _width = 64;
        [Range(1, 64)] [SerializeField] private int _height = 8;

        private ulong _numberOfClones;
        private byte _numberOfInstances;
        private GameObject _world;

        private List<GameObject> Chunks { get; set; }

        [Header("Параметры генератора")] [SerializeField]
        private int _seed;

        [SerializeField] [Range(0, 32)] private float _roughness = 2f;

        private WorldGenerator() { }

        protected internal override void Loading() {
            base.Loading();

            Chunks = new List<GameObject>();
            _world = new GameObject("World");

            _seed = _seed != 0 ? _seed : Random.Range(-1000, 1000);

            Data.Player.transform.position = new Vector3(_width * .5f, _height + 20, _width * .5f);

            Status = ManagerStatus.Started;
        }

        protected internal override void Update_() {
            Generator();
        }

        private void Generator() {
            var a = Mathf.FloorToInt(Data.Player.transform.position.x / WidthForChunk);
            var b = Mathf.FloorToInt(Data.Player.transform.position.z / WidthForChunk);
            for (var x = -3; x < 3; x++)
                for (var z = -3; z < 3; z++) {
                    var chunk = GetChunk(x + a, z + b);

                    if (!chunk)
                        MakeChunk(x + a, z + b);
                    else if (x == 3 || z == 3 || x == -3 || z == -3)
                        chunk.SetActive(false);
                    else
                        chunk.SetActive(true);
                }
        }

        public void MakeChunk(int deltax, int deltaz) {
            #region vars

            int deltaX = deltax * WidthForChunk, deltaZ = deltaz * WidthForChunk;
            var chunk = new GameObject($"Chunk {deltax}:{deltaz}").transform;

            var worldTransform = _world.transform;

            #endregion

            EvalPosAndClone(new Vector2Int(deltaX, deltaZ), chunk);

            chunk.SetParent(worldTransform);
            Chunks.Add(chunk.gameObject);
        }

        private async void EvalPosAndClone(Vector2Int delta, Transform chunk) {
            var noiseMap = Noise.GenerateNoiseMap(WidthForChunk, WidthForChunk, _roughness, _seed, 8, 1, 1, delta);

            for (var x = delta.x; x < delta.x + WidthForChunk; x++)
                for (var z = delta.y; z < delta.y + WidthForChunk; z++) {
                    var uppestPoint = Mathf.RoundToInt(_height * noiseMap[x - delta.x, z - delta.y]);

                    for (var y = 0; y <= uppestPoint; y++) {
                        var pos = new Vector3(x, y, z);
                        GenerateBlock(chunk, pos);
                    }

                    await BlocksPerTick();
                }
        }

        private async Task BlocksPerTick() {
            if (_numberOfInstances < 15) return;
            _numberOfInstances = 0;
            await Task.Delay(5);
        }

        private void GenerateBlock(Transform chunk, Vector3 pos) {
            bool make;

            do {
                var num = Random.Range(0, GameRegistry.BlockSpawnChance.Count);
                var genProps = GameRegistry.BlockSpawnChance[num];
                var chance = genProps.ChanceToSpawn;

                make = chance - 1 == Random.Range(0, chance);
                if (make)
                    PutBlock(genProps.Item as Block, pos, chunk);
            } while (!make);
        }

        /// <summary>
        /// Возвращает новый блок с характеристиками origGO
        /// </summary>
        /// <param name="origGo">Блок-предок</param>
        /// <param name="pos">Позиция для нового блока</param>
        /// <param name="chTransform">Чанк, куда следует внести новый блок</param>
        /// <returns></returns>
        public GameObject PutBlock(GameObject origGo, Vector3 pos, Transform chTransform) {
            if (origGo == null) throw new ArgumentNullException(nameof(origGo));

            var clone = Instantiate(origGo, pos, origGo.transform.rotation);
            var blockCom = Block.Get(origGo);

            clone.name = blockCom.FullName + ":" + _numberOfClones;
            _numberOfClones++;
            clone.tag = "Block";
            clone.transform.SetParent(chTransform);

            return clone;
        }

        public void ReplaceWith(GameObject from, GameObject to, Transform chTransform) {
            var pos = from.transform.position;
            PutBlock(to, pos, chTransform);
            Destroy(from);
        }

        public static GameObject GetChunk(int x, int z) =>
            Managers.FindByName(Managers.GetManager<WorldGenerator>().Chunks, $"Chunk {x}:{z}");
    }
}