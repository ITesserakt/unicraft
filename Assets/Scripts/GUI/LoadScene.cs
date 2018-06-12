using mc2.general;
using mc2.utils;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace mc2.ui {
    public class LoadScene : MonoBehaviour {
        [SerializeField] private Button _loadBut;
        [SerializeField] private Image _loadAmount;

        private void Start() {
            _loadBut.OnClickAsObservable()
                    .Subscribe(_ => {
                        _loadAmount.gameObject.SetActive(true);
                        LoadSceneAsync(1);
                    });
        }

        private void LoadSceneAsync(int num) {
            SceneManager.LoadSceneAsync(num).AsAsyncOperationObservable()
                        .Do(p => {
                            if(_loadAmount != null)
                                _loadAmount.fillAmount = p.progress * .9f;
                        })
                        .Subscribe(_ => Messenger.PublishEvent(this, GameEvents.ChangeScene));
        }
    }
}