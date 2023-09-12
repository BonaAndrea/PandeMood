using System;
using Slowdown;
using UnityEngine;

namespace Character.Character_Controller
{
    public class VCharacterSlowdown : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSlow;
        private TimeScaling _timeScaling;

        private bool _hasTimeScaling;
        private bool _audio;

        private void Start()
        {
            _timeScaling = FindObjectOfType<TimeScaling>();
            _hasTimeScaling = _timeScaling != null;
            _audio = audioSlow != null;

        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetButtonDown("Slowdown") && _hasTimeScaling)
            {
                _timeScaling.StartSlowdown();
                if (_audio)
                    audioSlow.Play();
            }
        }
    }
}