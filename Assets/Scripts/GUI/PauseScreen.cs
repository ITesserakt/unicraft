using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace mc2.ui {
    public sealed class PauseScreen : MonoBehaviour {

        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _settings;
        
        private GameObject _rButton;
        private GameObject _qButton;
        private GameObject _sButton;

        public static bool IsPause { get; private set; }

        private void Start() {

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
                          if (!IsPause) {
                              _panel.SetActive(true);
                              Time.timeScale = 0;

                              Cursor.lockState = CursorLockMode.None;
                              Cursor.visible = true;

                              IsPause = !IsPause;
                          }
                          else {
                              _panel.SetActive(false);
                              Time.timeScale = 1f;

                              Cursor.lockState = CursorLockMode.Locked;
                              Cursor.visible = false;

                              IsPause = !IsPause;
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
                              
            IsPause = !IsPause;
        }

        private void OnClickS() {
            _panel.SetActive(false);
            _settings.SetActive(true);
        }
    }
}