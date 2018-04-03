using System;
using mc2.general;
using UniRx;
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
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.id == GameEvents.ManagersInProgress)
                         .Subscribe(
                             msg => {
                                 var arg1 = Convert.ToInt32(((string) msg.data).Split(' ')[0]);
                                 var arg2 = Convert.ToInt32(((string) msg.data).Split(' ')[1]);
                                 OnManInProg(arg1, arg2);
                             }
                         );
            MessageBroker.Default
                         .Receive<Messenger>()
                         .Where(msg => msg.id == GameEvents.ManagersStarted)
                         .Subscribe(_ => OnStarted());
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