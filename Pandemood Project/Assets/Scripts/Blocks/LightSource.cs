using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class LightSource : MonoBehaviour
    {
        [SerializeField] private Light pointLight;
        private float _radius;
        private MeshRenderer _renderer;
        private readonly List<ShadowBlock> _shadowBLocks = new List<ShadowBlock>();
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        [SerializeField] private PressurePlate pressure;

        // Start is called before the first frame update
        void Start()
        {
            _radius = pointLight.range;
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material.EnableKeyword("_EMISSION");
            var objs = FindObjectsOfType<ShadowBlock>();
            var tmpPos = transform.position;
            tmpPos.z = 0;
            foreach (var obj in objs)
            {
                if (Vector3.Distance(tmpPos, obj.transform.position) <= _radius) _shadowBLocks.Add(obj);
            }

            if (pointLight.intensity != 0)
            {
                _renderer.material.SetColor(EmissionColor, Color.white);
                foreach (var shadow in _shadowBLocks)
                {
                    shadow.PressPlate();
                }
            }

            if (pointLight.intensity == 0)
            {
                pressure.ONPressure += TurnOnLight;
                pressure.ONRelease += TurnOffLight;
            }
            else
            {
                pressure.ONPressure += TurnOffLight;
                pressure.ONRelease += TurnOnLight;
            }
        }

        private void TurnOnLight(object sender, EventArgs args)
        {
            Debug.Log("Press");
            pointLight.intensity = 1;
            _renderer.material.SetColor (EmissionColor, Color.white);
            foreach (var shadow in _shadowBLocks)
            {
                shadow.PressPlate();
            }
        }

        private void TurnOffLight(object sender, EventArgs args)
        {
            pointLight.intensity = 0;
            _renderer.material.SetColor (EmissionColor, Color.black);
            foreach (var shadow in _shadowBLocks)
            {
                shadow.ReleasePlate();
            }
        }
    }
}