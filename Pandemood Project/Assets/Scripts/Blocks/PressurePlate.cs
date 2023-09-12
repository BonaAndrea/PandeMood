using System;
using UnityEngine;

namespace Blocks
{
    public class PressurePlate : MonoBehaviour
    {
        private int _nOfObject;
        public event EventHandler ONPressure, ONRelease;

        
        private void OnCollisionEnter()
        {
            _nOfObject++;
            if (_nOfObject <= 0) return;
            ONPressure?.Invoke(this, EventArgs.Empty);
        }

        private void OnCollisionExit()
        {
            _nOfObject--;
            if (_nOfObject > 0) return;
            ONRelease?.Invoke(this, EventArgs.Empty);
        }
    }
}