using System;
using UnityEngine;

namespace mc2.general
{
    public class Timer : MonoBehaviour
    {
        private DateTime timeBegin;
        public TimeSpan time { get; private set; }

        private void FixedUpdate()
        {
            time = DateTime.Now - timeBegin;
        }

        private void Start()
        {
            timeBegin = DateTime.Now;
        }
    }
}