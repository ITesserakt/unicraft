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

        protected internal virtual void Update_() { }

        public override string ToString() {
            return base.ToString().Split('(')[1].Split(')')[0].Split('.')[2];
        }
    }
}