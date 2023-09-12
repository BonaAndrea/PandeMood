using UnityEngine;

namespace Slowdown
{
    public class TimeScaling : MonoBehaviour
    {
        [SerializeField] private float duration = 2;
        [SerializeField] private float scaling = 0.5f;
        private float _time;


        private void Update()
        {
            if (_time > 0)
            {
                _time -= Time.deltaTime;
            }
        }

        public void StartSlowdown()
        {
            if (_time <= 0)
                _time = duration;
        }

        public float GetScaling()
        {
            if (_time > 0)
            {
                return scaling;
            }

            return 1;
        }
    }
}