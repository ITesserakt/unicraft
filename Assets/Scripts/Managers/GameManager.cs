using System;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mc2.managers
{
    public abstract class GameManager : MonoBehaviour {
        public ManagerStatus Status { get; protected internal set; } = ManagerStatus.Shutdown;
        public Exception Exception { get; private set; }

        protected internal virtual void Loading()
        {
            Status = ManagerStatus.Initializing;
        }

        protected static GameObject LoadAndCheckForNull(string gObjToLoad)
        {
            var g = GameObject.Find(gObjToLoad);
            if (g != null)
                return g;
            throw new NullReferenceException();
        }

        protected internal abstract void Update_();

        public override string ToString() {
            return base.ToString().Split('(')[1].Split(')')[0].Split('.')[2];
        }
    }
}