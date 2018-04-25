using System;
using System.Collections;
using System.Collections.Generic;
using mc2.mod;
using mc2.utils;
using UniRx;
using UnityEngine;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace mc2.managers {
    [DontLoadOnStatup]
    public class WorldGenerator : GameManager {
        public const byte Width = 16;

        [Range(1, 10)] [SerializeField] private int _countOfChunks = 4, _height = 8;
        private float[,] _noiseMap;

        private byte _numberOfClones, _numberOfInstances;
        private GameObject _world;

        public List<GameObject> World { get; private set; }
        public List<GameObject> Chunks { get; private set; }

        private WorldGenerator() { }

        protected internal override void Loading(GameManager manager) {
            base.Loading(this);

            World = new List<GameObject>();
            Chunks = new List<GameObject>();
            _noiseMap = new float[Width * _countOfChunks, Width * _countOfChunks];
            _world = new GameObject("World");

            var hud = GameObject.Find("HUD");
            if (!IsLoad(hud, "HUD")) return;

            Managers.Player.transform.position = new Vector3(_countOfChunks / 2 * 16 + 1, _height + 10,
                                                             _countOfChunks / 2 * 16 + 1);

            var random = Random.Range(0, int.MaxValue);
            _noiseMap = Noise.GenerateNoiseMap(10000, 10000, 25, random, 1, 1, 2, new Vector2());
            
            GenerateSpawnArea();

            Status = ManagerStatus.Started;
        }

        /*private void Generator() {
            var playerPos = Managers.Player.transform;
            var playerPosFChunk = new Vector2Int(Mathf.FloorToInt(playerPos.position.x / Width),
                                                 Mathf.FloorToInt(playerPos.position.y / Width));
            
            var nearFPlayerChunks = new[] {
                playerPosFChunk - Vector2Int.one,
                playerPosFChunk - Vector2Int.right,
                playerPosFChunk - new Vector2Int(1, -1),
                playerPosFChunk - Vector2Int.up,
                playerPosFChunk,
                playerPosFChunk - Vector2Int.down,
                playerPosFChunk - new Vector2Int(-1, 1),
                playerPosFChunk - Vector2Int.left,
                playerPosFChunk - new Vector2Int(-1, -1)
            };
                
            for (var i = 0; i < 9; i++)
                if (Managers.FindByName(Chunks, "Chunk " + nearFPlayerChunks[i].x + ":" + nearFPlayerChunks[i].y) ==
                    null) {
                    MakeChunk(nearFPlayerChunks[i].x, nearFPlayerChunks[i].y, _world.transform).ToObservable()
                                                                                               .Subscribe();
                }
        }*/

        private void GenerateSpawnArea() {
            for (var x = 0; x < 3; x++)
                for (var z = 0; z < 3; z++) {
                    StartCoroutine(MakeChunk(x, z, _world.transform));
                }
        }

        private IEnumerator MakeChunk(int deltax, int deltaz, Transform wTransform) {
            int deltaX = deltax * Width, deltaZ = deltaz * Width;

            var chunk = new GameObject(string.Format("Chunk {0}:{1}", deltax, deltaz));

            var regBlocks = GameRegistry.RegisteredBlocks;
            var blocks = new GameObject[regBlocks.Count];

            var i = 0;
            foreach (var key in regBlocks.Keys) {
                blocks[i] = new GameObject(key);
                i++;
            }

            for (var x = deltaX; x < deltaX + Width; x++)
                for (var z = deltaZ; z < deltaZ + Width; z++) {
                    var uppestPoint = Mathf.RoundToInt(_height * _noiseMap[x, z]);

                    for (var y = 0; y <= uppestPoint; y++) {
                        var pos = new Vector3(x, y, z);

                        if (y == 0) {
                            CloneTo(regBlocks["Bedrock"], pos, chunk.transform);
                        }
                        else if (y == uppestPoint) {
                            CloneTo(regBlocks["Grass"], pos, chunk.transform);
                        }
                        else {
                            CloneTo(regBlocks["Dirt"], pos, chunk.transform);
                        }

                        //TODO: добавить авто-условия для генерации мира
                    }
                }

            foreach (var block in blocks) {
                block.transform.SetParent(chunk.transform);
            }

            chunk.transform.SetParent(wTransform);
            Chunks.Add(chunk);

            if (_numberOfInstances > 60) {
                _numberOfInstances = 0;
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Возвращает новый блок с характеристиками origGO
        /// </summary>
        /// <param name="origGo">Блок-предок</param>
        /// <param name="pos">Позиция для нового блока</param>
        /// <param name="chTransform">Чанк, куда следует внести новый блок</param>
        /// <returns></returns>
        public GameObject CloneTo(GameObject origGo, Vector3 pos, Transform chTransform) {
            if (origGo == null) return null;

            var clone = Instantiate(origGo, pos, origGo.transform.rotation);
            clone.name = origGo.GetComponent<Block>().FullName + ":" + _numberOfClones;
            _numberOfClones++;
            clone.tag = clone.GetComponent<Block>().IsHarvest ? Managers.BlockTags[0] : Managers.BlockTags[1];
            clone.transform.SetParent(chTransform);
            World.Add(clone);
            return clone;
        }
    }
}
