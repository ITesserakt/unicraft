using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using mc2.general;
using UniRx;
using UnityEngine;

namespace mc2.managers {
    public enum ManagerStatus {
        Shutdown,
        Initializing,
        Started
    }

    [RequireComponent(typeof(WorldGenerator), typeof(StartupController), typeof(MakeDestroy))]
    public sealed class Managers : MonoBehaviour {
        public static readonly List<string> BlockTags = new List<string> {
            "BreakableBlock",
            "UnbreakableBlock"
        };

        private List<GameManager> _startSequence;
        
        public static GameObject Player { get; private set; }
        public static WorldGenerator WGenerator { get; private set; }
        public static MakeDestroy MkDest { get; private set; }
        public static WorldControl WControl { get; private set; }

        private void Awake() {
            // ReSharper disable once UnusedVariable
            var core = new Main();
            Player = GameObject.FindWithTag("Player");
            WGenerator = GetComponent<WorldGenerator>();
            MkDest = GetComponent<MakeDestroy>();
            WControl = Player.GetComponent<WorldControl>();

            _startSequence = new List<GameManager> {
                MkDest,
                WGenerator,
                WControl
            };
            
            Observable.FromCoroutine(StartupManagers)
                      .Subscribe(
                          _ => Debug.Log("All modules started")
                      );
        }

        private IEnumerator StartupManagers() {
            _startSequence.ForEach(
                manager => manager.Loading(manager)
            );

            yield return null;

            var numModules = _startSequence.Count;
            var numReady = 0;

            while (numReady < numModules) {
                var lastReady = numReady;
                numReady = 0;

                foreach (var manager in _startSequence)
                    switch (manager.Status) {
                        case ManagerStatus.Started:
                            numReady++;
                            break;
                        case ManagerStatus.Shutdown:
                            Debug.Log(string.Format("Something wrong in module {0}, in object {1}", manager,
                                                    manager.Exception));
                            break;
                    }

                if (numReady > lastReady) {
                    Debug.Log(string.Format("Loading process: {0}/{1}", numReady, numModules));
                    Messenger<int, int>.Broadcast(GameEvents.ManagersInProgress, numReady, numModules);
                }

                yield return new WaitForEndOfFrame();
            }

            Messenger.Broadcast(GameEvents.ManagersStarted);
        }

        public static GameObject FindByName(IEnumerable<GameObject> collection, string name) {
            var gameObjects = collection as GameObject[] ?? collection.ToArray();
            for (var i = 0; i < gameObjects.Count(); i++) {
                if (gameObjects[i] != null && gameObjects[i].name == name) {
                    return gameObjects[i];
                }
            }

            return null;
        }

        public static GameObject FindById(IEnumerable<GameObject> collection, uint id) {
            var gameObjects = collection as GameObject[] ?? collection.ToArray();
            for (var i = 0; i < gameObjects.Count(); i++) {
                if (gameObjects[i] != null && gameObjects[i].GetComponent<Block>().Id == id) {
                    return gameObjects[i];
                }
            }

            return null;
        }
    }
}
