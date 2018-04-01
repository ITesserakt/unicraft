using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mc2.managers
{
    public abstract class GameManager : MonoBehaviour
    {
        public ManagerStatus Status { get; protected set; }

        public Exception Exception { get; private set; }

        protected internal virtual void Loading(GameManager manager)
        {
            Debug.Log(string.Format("Module {0} still starting", manager));
            Status = ManagerStatus.Initializing;
        }

        protected bool IsLoad(Object obj, string n)
        {
            if (obj != null)
                return true;
            Status = ManagerStatus.Shutdown;
            Exception = new Exception("Object " + n + " not loaded");
            return false;
        }
    }
}