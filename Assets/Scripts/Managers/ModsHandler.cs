using System;
using System.IO;
using System.Reflection;
using mc2.utils;
using UnityEngine;

namespace mc2.managers {
    [DontLoadOnStatup]
    public class ModsHandler : GameManager {

        protected internal override void Loading() {
            base.Loading();

            var pluginsFolder = Data.PluginsFolder;

            if (!Directory.Exists(pluginsFolder))
                Directory.CreateDirectory(pluginsFolder);

            var dirs = Directory.GetFiles(pluginsFolder, "*.dll", SearchOption.AllDirectories);

            foreach (var file in dirs) {
                var mAssemly = Assembly.LoadFrom(file);
                var mClass = mAssemly.GetType(mAssemly.GetName().Name + ".Main");

                if (mClass?.GetInterface("IMod", true) == null) continue;
                var mClObj = Activator.CreateInstance(mClass, null, null);
                var pLMethod = mClass.GetMethod("PreLoad");
                var lMethod = mClass.GetMethod("Load");

                try {
                    pLMethod?.Invoke(mClObj, null);
                }
                catch (Exception ex) {
                    Debug.LogError(
                        $"В методе 'PreLoad' сборки {mAssemly.GetName().Name} зафиксирована ошибка ({ex.InnerException})");
                }

                try {
                    lMethod?.Invoke(mClObj, null);
                }
                catch (Exception ex) {
                    Debug.LogError(
                        $"В методе 'Load' сборки {mAssemly.GetName().Name} зафиксирована ошибка ({ex.Source})");
                }
            }

            Status = ManagerStatus.Started;
        }

        protected internal override void OnUpdate() { }

        private ModsHandler() { }
    }
}