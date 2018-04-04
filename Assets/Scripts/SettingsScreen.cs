using UnityEngine;

namespace mc2.general {
    public class SettingsScreen : MonoBehaviour {
        private static bool _isFullscreen = false;

        internal static void ToggleFullscreen() {
            _isFullscreen = !_isFullscreen;
            Screen.fullScreen = _isFullscreen;
        }

        internal static void SetQuality(int q) {
            QualitySettings.SetQualityLevel(q);
        }
    }
}