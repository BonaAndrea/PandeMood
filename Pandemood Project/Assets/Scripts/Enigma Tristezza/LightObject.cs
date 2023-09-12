using Blocks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enigma_Tristezza
{
    public class LightObject : MonoBehaviour
    {
        [FormerlySerializedAs("Illumination")] public GameObject illumination;
        public GameObject particles;
        [FormerlySerializedAs("AudioShadow")] public AudioSource audioShadow;
        [FormerlySerializedAs("DistanceIllumination")] public float distanceIllumination = 5f;
        private GameObject _player;
        private Renderer _renderer;
        private Collider _collider;
        private LightOnOff _lightOnOff;
        private ShadowBlock _shadowBlock;
        public bool illuminated;
        private bool _hasRenderer, _hasParticles;

        // Start is called before the first frame update
        private void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _renderer = GetComponent<Renderer>();
            _collider = GetComponent <Collider>();
            _lightOnOff = illumination.GetComponent<LightOnOff>();
            _shadowBlock = GetComponent<ShadowBlock>();
            _hasRenderer = _renderer != null;
            _hasParticles = particles != null;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_shadowBlock == null) return;
            if (_shadowBlock.GetActive()) //SCOMPARE OGGETTO, LUCE ACCESA
            {
                _collider.enabled = false;
                if(_hasRenderer)
                    _renderer.enabled = false;
                if(_hasParticles)
                    particles.SetActive(false);
                if (!illuminated)
                    audioShadow.Play();

                illuminated = true;
            }
            else //COMPARE OGGETTO, LUCE SPENTA
            {
                _collider.enabled = true;
                if (_hasRenderer)
                    _renderer.enabled = true;
                if (_hasParticles)
                    particles.SetActive(true);

                if (illuminated)
                    audioShadow.Play();
                illuminated = false;
            }
        }
    }
}
