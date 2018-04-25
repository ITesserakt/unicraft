using System;
using mc2.utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace mc2.managers {
    internal class StartupController : MonoBehaviour {
        [SerializeField] private Slider _progressBar;

        private void Awake() {
            _progressBar.gameObject.SetActive(true);
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.Id == GameEvents.ManagersInProgress)
                         .Subscribe(
                             m => _progressBar.value = Mathf.Clamp01(
                                 (float) (Convert.ToDecimal(m.Data[0]) / Convert.ToDecimal(m.Data[1])))
                         );
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.Id == GameEvents.ManagersStarted)
                         .Subscribe(_ => _progressBar.gameObject.SetActive(false));
        }
    }
}