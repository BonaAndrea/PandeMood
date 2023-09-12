using Slowdown;
using UnityEngine;

namespace Enigma_Tristezza
{
    public class RallentyTime : MonoBehaviour
    {
        public bool rallenty;
        private Animator _animator;
        private BubbleDropped _bubbleDropped;
        private BubbleLifebuoy _bubbleLifebuoy;
        private DropBubbleTube _dropBubbleTube;
        private NVBoids _boids;
        private float _startVelocity;
        [SerializeField] private float rallentySpeed = 0.3f;
        private TimeScaling _timeScaling;

        // Start is called before the first frame update
        private void Start()
        {
            _timeScaling = FindObjectOfType<TimeScaling>();
            if (GetComponent<Animator>() != null)
            {
                _animator = GetComponent<Animator>();
                _startVelocity = _animator.speed;
            }
            if (GetComponent<BubbleDropped>() != null)
            {
                _bubbleDropped = GetComponent<BubbleDropped>();
                _startVelocity = _bubbleDropped.speed;
            }

            if (GetComponent<BubbleLifebuoy>() != null)
            {
                _bubbleLifebuoy = GetComponent<BubbleLifebuoy>();
                _startVelocity = _bubbleLifebuoy.speed;
            }

            if (GetComponent<DropBubbleTube>() != null)
            {
                _dropBubbleTube = GetComponent<DropBubbleTube>();
                _startVelocity = _dropBubbleTube.timeDropBubble;
            }

            if (GetComponent<NVBoids>() != null)
            {
                _boids = GetComponent<NVBoids>();
                _startVelocity = _boids.birdSpeed;
            }
        }

        // Update is called once per frame
        private void Update()
        {

            if (_timeScaling.GetScaling() <= 0.9f)
            {
                rallenty = true;
                if (!(_bubbleDropped is null))
                    _bubbleDropped.speed = rallentySpeed;
                if (!(_animator is null))
                    _animator.speed = rallentySpeed;
                if (!(_bubbleLifebuoy is null))
                    _bubbleLifebuoy.speed = rallentySpeed;
                if (!(_dropBubbleTube is null))
                    _dropBubbleTube.timeDropBubble = rallentySpeed;
                if (!(_boids is null))
                    _boids.birdSpeed = rallentySpeed / 4;
            }
            else
            {
                rallenty = false;
                if (!(_bubbleDropped is null))
                    _bubbleDropped.speed = _startVelocity;
                if (!(_animator is null))
                    _animator.speed = _startVelocity;
                if (!(_bubbleLifebuoy is null))
                    _bubbleLifebuoy.speed = _startVelocity;
                if (!(_dropBubbleTube is null))
                    _dropBubbleTube.timeDropBubble = _startVelocity;
                if (!(_boids is null))
                    _boids.birdSpeed = _startVelocity;
            }
        }
    }
}