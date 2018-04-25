using System;
using System.IO;
using System.Reflection;
using UniRx;
using UnityEngine;

namespace mc2.managers {
    [DontLoadOnStatup]
    public class ModsHandler : GameManager {

        protected internal override void Loading(GameManager manager) {
            base.Loading(this);

            var appFolder = Managers.AppFolder;

            if (!Directory.Exists(appFolder + @"\Plugins"))
                Directory.CreateDirectory(appFolder + @"\Plugins");

            var dirs = Directory.GetFiles(appFolder + @"\Plugins", "*.dll", SearchOption.AllDirectories);

            foreach (var file in dirs) {
                var mAssemly = Assembly.LoadFrom(file);
                var mClass = mAssemly.GetType(mAssemly.GetName().Name + ".Main");

                if (mClass.GetInterface("IMod", true) == null) continue;
                var mClObj = Activator.CreateInstance(mClass, null, null);
                var pLMethod = mClass.GetMethod("PreLoad");
                var lMethod = mClass.GetMethod("Load");

                Debug.Assert(pLMethod != null, "pLMethod != null");
                pLMethod.Invoke(mClObj, null);
                Debug.Assert(lMethod != null, "lMethod != null");
                lMethod.Invoke(mClObj, null);
            }

            Status = ManagerStatus.Started;
        }

        private ModsHandler() { }
    }
}