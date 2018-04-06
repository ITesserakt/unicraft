using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using mc2.general;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace mc2.managers {
    public class WorldGenerator : GameManager {
        public const byte Width = 16;

        [Range(1, 10)] [SerializeField] private int _countOfChunks = 4, _height = 8;
        private float[,] _noiseMap;

        private uint _numberOfClones, _numberOfInstances;
        private GameObject _world;

        public List<GameObject> Voxels { get; private set; }
        public List<GameObject> World { get; private set; }
        public List<GameObject> Chunks { get; private set; }
        public int CountOfChunks { get; private set; }

        protected internal override void Loading(GameManager manager) {
            base.Loading(this);

            CountOfChunks = _countOfChunks;
            Voxels = new List<GameObject>();
            World = new List<GameObject>();
            Chunks = new List<GameObject>();
            _noiseMap = new float[Width * _countOfChunks, Width * _countOfChunks];
            _world = new GameObject("World");

            var hud = GameObject.Find("HUD");
            if (!IsLoad(hud, "HUD")) return;

            Voxels.AddRange(new[] {Main.Dirt, Main.Bedrock});
            Managers.Player.transform.position = new Vector3(_countOfChunks / 2 * 16 + 1, _height + 10,
                                                             _countOfChunks / 2 * 16 + 1);

            var random = Random.Range(0, int.MaxValue);
            /*Observable.Start(() => Noise.GenerateNoiseMap(1000, 1000, 25, random, 1, 1, 2,
                                                          new Vector2()), Scheduler.ThreadPool)
                      .ObserveOnMainThread(MainThreadDispatchType.Update)
                      .Subscribe(ret => _noiseMap = ret);*/

            _noiseMap = Noise.GenerateNoiseMap(1000, 1000, 25, random, 1, 1, 2, new Vector2());

            GenerateSpawnArea();
            
            Status = ManagerStatus.Started;
        }

        private void Generator() {
            var playerPos = Managers.Player.transform;
            var playerPosFChunk = new Vector2Int(Mathf.FloorToInt(playerPos.position.x / Width),
                                                 Mathf.FloorToInt(playerPos.position.y / Width));
            
            var nearFPlayerChunks = new Vector2Int[9] {
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
        }

        private void GenerateSpawnArea() {
            for (var x = 0; x < _countOfChunks; x++)
                for (var z = 0; z < _countOfChunks; z++) {
                    MakeChunk(x, z, _world.transform).ToObservable()
                                                    .Subscribe();
                }
        }

        private IEnumerator MakeChunk(int deltax, int deltaz, Transform wTransform) {
            int deltaX = deltax * Width, deltaZ = deltaz * Width;
            var chunk = new GameObject(string.Format("Chunk {0}:{1}", deltax, deltaz));

            for (var x = deltaX; x < deltaX + Width; x++)
                for (var z = deltaZ; z < deltaZ + Width; z++) {
                    var uppestPoint = Mathf.Round(_height * _noiseMap[x, z]);

                    for (var y = 0; y <= uppestPoint; y++) {
                        var pos = new Vector3(x, y, z);

                        if (y == 0)
                            ClonePlace(Managers.FindByName(Voxels, "Bedrock"), pos, chunk.transform);

                        if (y != 0)
                            ClonePlace(Managers.FindByName(Voxels, "Dirt"), pos, chunk.transform);
                        
                        //TODO: добавить авто-условия для генерации мира
                    }
                }

            chunk.transform.SetParent(wTransform);
            Chunks.Add(chunk);

            yield return null;
        }

        /// <summary>
        /// Возвращает новый блок с характеристиками origGO
        /// </summary>
        /// <param name="origGo">Блок-предок</param>
        /// <param name="pos">Позиция для нового блока</param>
        /// <param name="chTransform">Чанк, куда следует внести новый блок</param>
        /// <returns></returns>
        public GameObject ClonePlace(GameObject origGo, Vector3 pos, Transform chTransform) {
            if (origGo == null) return null;
            var clone = Instantiate(origGo, pos, origGo.transform.rotation);
            clone.name = origGo.GetComponent<Block>().FullName + ":" + _numberOfClones;
            _numberOfClones++;
            clone.tag = clone.GetComponent<Block>().IsHarvest ? Managers.BlockTags[1] : Managers.BlockTags[0];
            clone.transform.SetParent(chTransform);
            World.Add(clone);
            return clone;
        }
    }
}
