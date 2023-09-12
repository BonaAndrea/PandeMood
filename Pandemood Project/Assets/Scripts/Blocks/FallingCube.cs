using Slowdown;
using UnityEngine;

namespace Blocks
{
    public class FallingCube : MonoBehaviour
    {
        private TimeScaling _timeScaling;

        private bool _falling;
        private Vector3 _speed = new Vector3(0, 0, 0);
        // Start is called before the first frame update
        private void Start()
        {
            _timeScaling = FindObjectOfType<TimeScaling>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_falling) return;
            _speed += Physics.gravity *  (Time.deltaTime * _timeScaling.GetScaling());
            transform.position += _speed * (Time.deltaTime * _timeScaling.GetScaling());
        }


        private void OnCollisionEnter(Collision other)
        {
            if (!_falling && other.gameObject.CompareTag("Player"))
            {
                _falling = true;
            }
        
        }
    }
}
