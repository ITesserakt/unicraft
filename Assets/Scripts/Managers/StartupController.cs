using System;
using mc2.general;
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
                             msg => _progressBar.value =
                                 (float) (Convert.ToDecimal(msg.Data[0]) / Convert.ToDecimal(msg.Data[1]))
                         );
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.Id == GameEvents.ManagersStarted)
                         .Subscribe(_ => _progressBar.gameObject.SetActive(false));
        }
    }
}