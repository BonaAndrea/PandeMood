using System;
using Slowdown;
using UnityEngine;

namespace Blocks
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private PressurePlate plate;
        [SerializeField] private bool useTimeScaling = true;

        private Animator _animator;
        private TimeScaling _timeScaling;

        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int TimeMultiplier = Animator.StringToHash("TimeMultiplier");

        // Start is called before the first frame update
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _timeScaling = FindObjectOfType<TimeScaling>();
            plate.ONPressure += OpenDoor;
            plate.ONRelease += CloseDoor;
            
        }

        private void Update()
        {
            if(!(_timeScaling is null) && useTimeScaling)
                _animator.SetFloat(TimeMultiplier, _timeScaling.GetScaling());
        }

        private void OpenDoor(object sender, EventArgs args)
        {
            _animator.SetBool(Open, true);
        }

        private void CloseDoor(object sender, EventArgs args)
        {
            _animator.SetBool(Open, false);
        }

    }
}
