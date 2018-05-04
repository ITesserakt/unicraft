using System.Collections;
using System.Collections.Generic;
using System.Linq;
using mc2.general;
using mc2.mod;
using UniRx;
using UnityEngine;

namespace mc2.managers {
    public enum ManagerStatus {
        Shutdown,
        Initializing,
        Started
    }

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
        public static ModsHandler MHandler { get; private set; }

        private void Awake() {
            
            Player = GameObject.FindWithTag("Player");
            
            WGenerator = GetComponent<WorldGenerator>();
            MkDest = GetComponent<MakeDestroy>();
            WControl = Player.GetComponent<WorldControl>();
            MHandler = GetComponent<ModsHandler>();

            _startSequence = new List<GameManager> {
                MHandler,
                WGenerator,
                MkDest,
                WControl
            };

            Observable.FromCoroutine(StartupManagers)
                      .Subscribe(_ => Debug.Log("All modules started"));
        }

        private IEnumerator StartupManagers() {
            _startSequence.ForEach(
                manager => manager.Loading(manager)
            );

            yield return null;

            var numModules = _startSequence.Count;
            var numReady = 0;

            while (numReady < numModules) {
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


                Debug.Log(string.Format("Loading process: {0}/{1}", numReady, numModules));
                MessageBroker.Default
                             .Publish(Messenger.Create(this, GameEvents.ManagersInProgress, numReady, numModules));


                yield return new WaitForEndOfFrame();
            }

            MessageBroker.Default
                         .Publish(Messenger.Create(this, GameEvents.ManagersStarted, null));
        }

        public static GameObject FindByName(IEnumerable<GameObject> collection, string name) {
            return collection.FirstOrDefault(g => g != null && g.name == name);
        }

        public static GameObject FindById(IEnumerable<GameObject> collection, uint id) {
            return collection.FirstOrDefault(g => g != null && g.GetComponent<Block>().Id == id);
        }
    }
}
