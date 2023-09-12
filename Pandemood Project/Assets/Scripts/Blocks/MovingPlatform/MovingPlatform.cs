using Slowdown;
using UnityEngine;

namespace Blocks.MovingPlatform
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private float leftDistance = 1, rightDistance = 1, speed = 1;
        private TimeScaling _timeScaling;
        private Vector3 _startPosition;

        private bool _isMovingRight;
        
        // Start is called before the first frame update
        private void Start()
        {
            _startPosition = transform.position;
            _timeScaling = FindObjectOfType<TimeScaling>();
        }

        // Update is called once per frame
        private void Update()
        {
            var tmp = transform.position;
            if (_isMovingRight)
            {
                if (tmp.x - _startPosition.x >= rightDistance)
                {
                    _isMovingRight = !_isMovingRight;
                    return;
                }

                tmp.x += speed * Time.deltaTime * _timeScaling.GetScaling();
                transform.position = tmp;
                
            }
            else
            {
                if (+
                    tmp.x - _startPosition.x <= -leftDistance)
                {
                    _isMovingRight = true;
                    return;
                }
                tmp.x -= speed * Time.deltaTime * _timeScaling.GetScaling();
                transform.position = tmp;
            }
            
        }

        
    }
}
