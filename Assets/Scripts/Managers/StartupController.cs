using mc2.general;
using UnityEngine;
using UnityEngine.UI;

namespace mc2.managers
{
    public class StartupController : MonoBehaviour
    {
        [SerializeField] private Slider _progressBar;

        private void Awake()
        {
            _progressBar.gameObject.SetActive(true);
            Messenger<int, int>.AddListener(GameEvents.ManagersInProgress, OnManInProg);
            Messenger.AddListener(GameEvents.ManagersStarted, OnStarted);
        }

        private void OnDestroy()
        {
            Messenger<int, int>.RemoveListener(GameEvents.ManagersInProgress, OnManInProg);
            Messenger.RemoveListener(GameEvents.ManagersStarted, OnStarted);
        }

        private void OnManInProg(int arg1, int arg2)
        {
            var progress = arg1 / (float) arg2;
            _progressBar.value = progress;
        }

        private void OnStarted()
        {
            _progressBar.gameObject.SetActive(false);
        }
    }
}