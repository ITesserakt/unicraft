using System;
using UnityEngine;

namespace mc2.mod
{
    public interface IMod {
        ModState State { get; set; }
        
        /// <summary>
        /// Используйте этот метод для введения новых переменных
        /// </summary>
        void PreLoad();

        /// <summary>
        /// Иользуйте это для введения обработчиков событий
        /// </summary>
        void Load();
    }

    public enum ModState {
        PreLoading,
        Loading,
        Error,
        SwitchedOff
    }
}