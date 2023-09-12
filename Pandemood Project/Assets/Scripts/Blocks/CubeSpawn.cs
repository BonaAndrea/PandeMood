using System;
using UnityEngine;

namespace Blocks
{
    public class CubeSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject myPrefab;
        private GameObject _cube;
        [SerializeField] private PressurePlate plate;
        // Start is called before the first frame update
        void Start()
        {
            if (_cube == null)
            {
                _cube = Instantiate(myPrefab, transform.position, Quaternion.identity);
            }
            plate.ONPressure += SpawnCube;
        }

        private void SpawnCube(object sender, EventArgs args)
        {
            if(_cube != null){
                _cube.transform.position = new Vector3(100000000.0f,10000000.0f,10000000.0f);
                Destroy(_cube, 0.2f);
            }
            _cube = Instantiate(myPrefab, transform.position, Quaternion.identity);
        }
    }
}
