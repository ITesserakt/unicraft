using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace mc2.ui {
    public sealed class PauseScreen : MonoBehaviour {

        [SerializeField] private GameObject _pause;
        [SerializeField] private GameObject _settings;
        
        private GameObject _rButton;
        private GameObject _qButton;
        private GameObject _sButton;

        public static ReactiveProperty<bool> IsPause;

        private void Start() {

            IsPause = new ReactiveProperty<bool>(false);

            IsPause.Subscribe(_ => FirstPersonController.IsPause = IsPause.Value);

            _rButton = _pause.transform.Find("Resume").gameObject;
            _qButton = _pause.transform.Find("Quit").gameObject;
            _sButton = _pause.transform.Find("Settings").gameObject;

            _rButton.GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_ => OnClickR());

            _qButton.GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_ => OnClickQ());

            _sButton.GetComponent<Button>().OnClickAsObservable()
                    .Subscribe(_ => OnClickS());
            
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyUp(KeyCode.Escape))
                      .Subscribe(_ => {
                          if (_settings.activeSelf) return;
                          if (!IsPause.Value) {
                              _pause.gameObject.SetActive(true);
                              Time.timeScale = 0;

                              Cursor.lockState = CursorLockMode.None;
                              Cursor.visible = true;

                              IsPause.Value = !IsPause.Value;
                          }
                          else {
                              _pause.gameObject.SetActive(false);
                              Time.timeScale = 1;

                              Cursor.lockState = CursorLockMode.Locked;
                              Cursor.visible = false;

                              IsPause.Value = !IsPause.Value;
                          }
                      });
        }

        private void OnClickQ() {
            Application.Quit();
        }

        private void OnClickR() {
            _pause.gameObject.SetActive(false);
            Time.timeScale = 1;
                              
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
                              
            IsPause.Value = !IsPause.Value;
        }

        private void OnClickS() {
            _pause.gameObject.SetActive(false);
            _settings.SetActive(true);
        }
    }
}