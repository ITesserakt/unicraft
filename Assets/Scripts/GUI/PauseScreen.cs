using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace mc2.ui {
    public sealed class PauseScreen : MonoBehaviour {

        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _settings;
        
        private GameObject _rButton;
        private GameObject _qButton;
        private GameObject _sButton;

        public static ReactiveProperty<bool> IsPause;

        private void Start() {

            IsPause = new ReactiveProperty<bool>(false);

            IsPause.Subscribe(_ => FirstPersonController.IsPause = IsPause.Value);

            _rButton = _panel.transform.Find("Resume").gameObject;
            _qButton = _panel.transform.Find("Quit").gameObject;
            _sButton = _panel.transform.Find("SettingsB").gameObject;

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
                              _panel.SetActive(true);
                              Time.timeScale = 0;

                              Cursor.lockState = CursorLockMode.None;
                              Cursor.visible = true;

                              IsPause.Value = !IsPause.Value;
                          }
                          else {
                              _panel.SetActive(false);
                              Time.timeScale = 1f;

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
            _panel.SetActive(false);
            Time.timeScale = 1f;
                              
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
                              
            IsPause.Value = !IsPause.Value;
        }

        private void OnClickS() {
            _panel.SetActive(false);
            _settings.SetActive(true);
        }
    }
}