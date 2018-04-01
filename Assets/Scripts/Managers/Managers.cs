using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using mc2.general;
using UnityEngine;

namespace mc2.managers
{
    public enum ManagerStatus
    {
        Shutdown,
        Initializing,
        Started
    }

    [RequireComponent(typeof(WorldGenerator), typeof(StartupController), typeof(MakeDestroy))]
    public sealed class Managers : MonoBehaviour
    {
        public static readonly List<string> BlockTags = new List<string>
        {
            "BreakableBlock",
            "UnbreakableBlock"
        };

        private List<GameManager> _startSequence;
        public static GameObject Player { get; private set; }
        public static WorldGenerator WGenerator { get; private set; }
        public static MakeDestroy MkDest { get; private set; }

        private void Awake()
        {
            // ReSharper disable once UnusedVariable
            var core = new Main();
            Player = GameObject.FindWithTag("Player");
            WGenerator = GetComponent<WorldGenerator>();
            MkDest = GetComponent<MakeDestroy>();

            _startSequence = new List<GameManager>
            {
                WGenerator,
                MkDest
            };
            StartCoroutine(StartupManagers());
        }

        private IEnumerator StartupManagers()
        {
            foreach (var manager in _startSequence) manager.Loading(manager);

            yield return null;

            var numModules = _startSequence.Count;
            var numReady = 0;

            while (numReady < numModules)
            {
                var lastReady = numReady;
                numReady = 0;

                foreach (var manager in _startSequence)
                    switch (manager.Status)
                    {
                        case ManagerStatus.Started:
                            numReady++;
                            break;
                        case ManagerStatus.Shutdown:
                            Debug.Log(string.Format("Something wrong in module {0}, in object {1}", manager,
                                manager.Exception));
                            break;
                    }

                if (numReady > lastReady)
                {
                    Debug.Log(string.Format("Loading process: {0}/{1}", numReady, numModules));
                    Messenger<int, int>.Broadcast(GameEvents.ManagersInProgress, numReady, numModules);
                }

                yield return new WaitForEndOfFrame();
            }

            Debug.Log("All modules started");
            Messenger.Broadcast(GameEvents.ManagersStarted);
        }

        public static GameObject FindByName(IEnumerable<GameObject> collection, string name)
        {
            return collection.FirstOrDefault(item => item != null && item.name == name);
        }

        public static GameObject FindById(IEnumerable<GameObject> collection, uint id)
        {
            return collection.FirstOrDefault(gameObject =>
                gameObject != null && gameObject.GetComponent<Block>().Id == id);
        }
    }
}
