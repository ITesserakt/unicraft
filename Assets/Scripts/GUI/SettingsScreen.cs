using mc2.managers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace mc2.ui {
    public class SettingsScreen : MonoBehaviour {
        
        private static bool _isFullscreen;
        
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject _fps;
        
        private Toggle _fullscreenTo;
        private Dropdown _qualityDl;
        private Slider _soundS;
        private Button _backB;
        private AudioSource _sound;
        private Toggle _showFps;

        private void Start() {

            _fullscreenTo = transform.Find("FullscreenT").Find("FullscreenTo").GetComponent<Toggle>();
            _fullscreenTo.OnValueChangedAsObservable()
                         .Subscribe(_ => ToggleFullscreen());


            _qualityDl = transform.Find("QualityT").Find("QualityDL").GetComponent<Dropdown>();
            _qualityDl.onValueChanged.AsObservable()
                      .Subscribe(SetQuality);
            SetQuality(_qualityDl.value);

            
            if (Managers.Player != null) {
                _sound = Managers.Player.GetComponent<AudioSource>();
                _soundS = transform.Find("SoundT").Find("SoundS").GetComponent<Slider>();
                _soundS.onValueChanged.AsObservable()
                       .Subscribe(x => _sound.volume = x / 100);
                _sound.volume = _soundS.value / 100;
            }


            _backB = transform.Find("BackB").GetComponent<Button>();
            _backB.OnClickAsObservable()
                  .Subscribe(_ => {
                      gameObject.SetActive(false);
                      _panel.SetActive(true);
                  });


            _showFps = transform.Find("ShowFPST").Find("ShowFPSTo").GetComponent<Toggle>();
            _showFps.OnValueChangedAsObservable()
                    .Subscribe(_fps.SetActive);
        }

        private static void ToggleFullscreen() {
            _isFullscreen = !_isFullscreen;
            Screen.fullScreen = !_isFullscreen;
        }

        private static void SetQuality(int q) {
            QualitySettings.SetQualityLevel(q);
        }
    }
}