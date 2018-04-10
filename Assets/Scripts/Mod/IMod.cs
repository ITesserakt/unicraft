using System;
using UnityEngine;

namespace mc2.mod
{
    public interface IMod {
        void OnRightClick(RaycastHit hit, Transform block);
        void OnLeftClick(RaycastHit hit);

        /// <summary>
        /// Используйте этот метод для введения новых переменных
        /// </summary>
        void PreLoad();

        /// <summary>
        /// Иользуйте это для введения обработчиков событий
        /// </summary>
        void Load();
    }
}