using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using mc2.mod;
using mc2.utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace mc2.managers {
    public enum ManagerStatus {
        Shutdown,
        Initializing,
        Started
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal class DontLoadOnStatupAttribute : Attribute {
    }

    public sealed class Managers : MonoBehaviour {
        public static readonly string[] BlockTags = {
            "BreakableBlock",
            "UnbreakableBlock"
        };

        public static string AppFolder;

        public static List<GameManager> StartSequence { get; private set; }

        public static GameObject Player { get; private set; }

        private void Awake() {
            
            DontDestroyOnLoad(gameObject);

#if UNITY_STANDALONE_LINUX
            AppFolder = @"~/.config/MC2";
#endif
#if UNITY_STANDALONE_WIN
            AppFolder = @"C://Games/MC2";
#endif
            StartSequence = new List<GameManager>();
            Player = GameObject.FindWithTag("Player");
            

            var managers = GetComponents<GameManager>();
            foreach (var manager in managers) {
                var attr = Attribute.IsDefined(manager.GetType(), typeof(DontLoadOnStatupAttribute));
                var curScene = SceneManager.GetActiveScene().buildIndex == 0;
                if (!attr || !curScene)
                    StartSequence.Add(manager);
            }

            Observable.FromCoroutine(StartupManagers)
                      .Subscribe(_ => Debug.Log("All modules started"));

            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.Id == GameEvents.ChangeScene)
                         .Subscribe(msg => Awake());

            Observable.EveryUpdate()
                .Subscribe(_ => {
                          foreach (var manager in StartSequence) {
                              manager.Update_();
                          }
                      });
        }

        private IEnumerator StartupManagers() {
            
            LoadManagers();

            yield return new WaitForEndOfFrame();

            var numModules = StartSequence.Count;
            var numReady = 0;

            while (numReady < numModules) {
                numReady = 0;

                foreach (var manager in StartSequence)
                    switch (manager.Status) {
                        case ManagerStatus.Started:
                            numReady++;
                            break;
                        case ManagerStatus.Shutdown:
                            Debug.Log(string.Format("Что-то не так в модуле {0}, с объектом {1}", manager,
                                                    manager.Exception.Message));
                            break;
                        case ManagerStatus.Initializing:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                Debug.Log(string.Format("Процесс загрузки: {0}/{1}", numReady, numModules));
                MessageBroker.Default
                             .Publish(Messenger.Create(this, GameEvents.ManagersInProgress, numReady, numModules));


                yield return new WaitForEndOfFrame();
            }

            MessageBroker.Default
                         .Publish(Messenger.Create(this, GameEvents.ManagersStarted, null));
        }

        private static void LoadManagers() {
            foreach (var manager in StartSequence) {
                if (manager == null) continue;

                var watch = Stopwatch.StartNew();
                manager.Loading(manager);
                watch.Stop();

                Debug.Log("Модуль " + manager + " загружен за " +
                          watch.Elapsed.TotalSeconds + " секунд");
            }
        }

        public static GameObject FindByName(IEnumerable<GameObject> collection, string name) {
            return collection.FirstOrDefault(g => g != null && g.name == name);
        }

        public static GameObject FindById(IEnumerable<GameObject> collection, uint id) {
            return collection.FirstOrDefault(g => g != null && g.GetComponent<Block>().Id == id);
        }
    }
}
