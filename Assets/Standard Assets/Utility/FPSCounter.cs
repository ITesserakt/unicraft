using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace UnityStandardAssets.Utility {

    public class FPSCounter : MonoBehaviour {
        [SerializeField] private float _frequency;
		private bool isOn = true;
        [SerializeField] private Text _fpsText;
        
        public static int FramesPerSec { get; protected set; }

        private void OnEnable() {
            Fps();
        }

        private async void Fps() {
            while(isOn) {
                // Capture frame-per-second
                var lastFrameCount = Time.frameCount;
                var lastTime = Time.realtimeSinceStartup;
                await Task.Delay((int)(_frequency * 1000));
                var timeSpan = Time.realtimeSinceStartup - lastTime;
                var frameCount = Time.frameCount - lastFrameCount;

                // Display it
                FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
				if (_fpsText != null)
					_fpsText.text = $"{FramesPerSec} fps";
            }
        }

        private void OnDestroy() {
			isOn = false;
        }
    }
}