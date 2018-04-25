using mc2.utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace mc2.ui {
    public class LoadScene : MonoBehaviour {
        [SerializeField] private Button _loadBut;
        [SerializeField] private Scrollbar _progressBar;

        private void Start() {
            _loadBut.OnClickAsObservable()
                    .Subscribe(_ => {
                        _progressBar.gameObject.SetActive(true);
                        LoadSceneAsync(1);
                    });
        }

        private void LoadSceneAsync(int num) {
            SceneManager.LoadSceneAsync(num).AsAsyncOperationObservable()
                        .Do(p => _progressBar.size = p.progress * .9f)
                        .Subscribe(_ => MessageBroker.Default.Publish(Messenger.Create(this, GameEvents.ChangeScene)));
        }
    }
}