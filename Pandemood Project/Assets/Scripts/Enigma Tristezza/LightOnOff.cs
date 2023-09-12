using UnityEngine;
using UnityEngine.Serialization;

namespace Enigma_Tristezza
{
    public class LightOnOff : MonoBehaviour
    {
        [FormerlySerializedAs("DistanceIllumination")]
        public float distanceIllumination = 5f;

        public bool lightOn;
        [FormerlySerializedAs("AudioLightOn")] public AudioSource audioLightOn;

        private GameObject _illuminationTexture;
        private Renderer _renderer, _illuminationRenderer;

        // Start is called before the first frame update
        private void Start()
        {
            transform.localScale = new Vector3(distanceIllumination, distanceIllumination, distanceIllumination);
            _renderer = GetComponent<Renderer>();
            _renderer.enabled = false;
            _illuminationTexture = transform.GetChild(0).gameObject;
            _illuminationRenderer = _illuminationTexture.GetComponent<Renderer>();
            _illuminationRenderer.enabled = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (lightOn) //spegni
            {
                lightOn = false;
                _renderer.enabled = false;
                _illuminationRenderer.enabled = false;
            }
            else //accendi
            {
                lightOn = true;
                _renderer.enabled = true;
                _illuminationRenderer.enabled = true;
                audioLightOn.Play();
            }
        }
    }
}