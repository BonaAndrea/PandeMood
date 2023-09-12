using System;
using UnityEngine;

namespace Blocks
{
    public class Ladder : MonoBehaviour
    {
        private MeshRenderer _renderer;

        private BoxCollider _boxCollider;

        [SerializeField] private PressurePlate plate;
        // Start is called before the first frame update
        void Start()
        {
            plate.ONPressure += Spawn;
            _renderer = GetComponent<MeshRenderer>();
            _renderer.enabled = false;
            _boxCollider = GetComponent<BoxCollider>();
            _boxCollider.enabled = false;
        }

        

        private void Spawn(object sender, EventArgs args)
        {
            _renderer.enabled = true;
            _boxCollider.enabled = true;
        }
    }
}
