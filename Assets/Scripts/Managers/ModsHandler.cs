using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace mc2.managers {
    public class ModsHandler : GameManager {
        private readonly string _appFolder = @"c://Games/MC2";

        protected internal override void Loading(GameManager manager) {
            base.Loading(manager);

            if (!Directory.Exists(_appFolder + @"\Plugins"))
                Directory.CreateDirectory(_appFolder + @"\Plugins");

            var dirs = Directory.GetFiles(_appFolder + @"\Plugins", "*.dll", SearchOption.AllDirectories);

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
    }
}