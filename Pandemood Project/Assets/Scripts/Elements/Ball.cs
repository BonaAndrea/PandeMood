using Slowdown;
using UnityEngine;

namespace Elements
{
    public class Ball : MonoBehaviour
    {

        private TimeScaling _timeScaling;

        private Animator _animator;

        private static readonly int Speed = Animator.StringToHash("Speed");

        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            _timeScaling = FindObjectOfType<TimeScaling>();
        }

        // Update is called once per frame
        void Update()
        {
            _animator.SetFloat(Speed, _timeScaling.GetScaling());
        }
    }
}
