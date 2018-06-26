using System;
using System.Collections.Generic;
using System.Linq;
using mc2.general;
using mc2.utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace mc2.managers {

    public sealed class Managers : MonoBehaviour {
        
        private static List<GameManager> StartSequence { get; } = new List<GameManager>();

        private void Awake() {
            DontDestroyOnLoad(gameObject);
            StartSequence.Clear();
            ManagersToLoad();

            StartupManagers();

            MessageBroker.Default.Receive<Messenger>()
                         .Where(msg => msg.Id == GameEvents.ChangeScene)
                         .Subscribe(_ => Awake());
        }

        private void Update() {
            StartSequence.ForEach(m => m.OnUpdate());
        }

        public static T GetManager<T>() where T : GameManager
        {
            var manager = StartSequence.First(m => m is T).To<T>();
            if (manager == null)
                throw new InvalidOperationException("Данный менеджер либо недоступен, либо несуществует");

            return manager;
        }

        private void ManagersToLoad() {
            var result = from manager in GetComponents<GameManager>()
                         where !Attribute.IsDefined(manager.GetType(), typeof(DontLoadOnStatupAttribute))
                         || SceneManager.GetActiveScene().name != "Load_scene"
                         select manager;

            StartSequence.AddRange(result);
        }

        private void StartupManagers() {

            LoadManagers();

            var numModules = StartSequence.Count;
            var numReady = 0;

            while (numReady < numModules) {
                numReady = 0;

                for (var i = 0; i < StartSequence.Count; i++) {
                    var manager = StartSequence[i];
                    numReady = Switch(numReady, manager);
                }

                MessageBroker.Default.Publish(
                    Messenger.Create(this, GameEvents.ManagersInProgress, numReady, numModules));
            }

            MessageBroker.Default.Publish(
                Messenger.Create(this, GameEvents.ManagersStarted));
        }

        private static int Switch(int numReady, GameManager manager) {
            switch (manager.Status) {
                case ManagerStatus.Started:
                    numReady++;
                    break;
                case ManagerStatus.Shutdown:
                    break;
                case ManagerStatus.Initializing:
                    break;
                case ManagerStatus.Exception:
                    Debug.LogError($"Что-то не так в модуле {manager}, с объектом {manager.Exception.Message}");
                    manager.Status = ManagerStatus.Shutdown;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return numReady;
        }

        private static void LoadManagers() {
            StartSequence.ForEach(m => m.Loading());
        }

        public static GameObject FindByName(IEnumerable<GameObject> collection, string forename) {
            return collection.FirstOrDefault(go => go != null && go.name == forename);
        }
    }
}