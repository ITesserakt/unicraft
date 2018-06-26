using mc2.general;
using mc2.utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace mc2.ui {
    public class LoadScene : MonoBehaviour {

        public void LoadSceneAsync(int num) {
            SceneManager.LoadSceneAsync(num).AsAsyncOperationObservable()
                        .Subscribe(_ => PubEvChange());
        }

        private void PubEvChange() {
            MessageBroker.Default.Publish(Messenger.Create(this, GameEvents.ChangeScene));
        }
    }
}