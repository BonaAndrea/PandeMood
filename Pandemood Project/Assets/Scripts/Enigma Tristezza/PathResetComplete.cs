using UnityEngine;
using UnityEngine.Serialization;

namespace Enigma_Tristezza
{
    public class PathResetComplete : MonoBehaviour
    {
        [FormerlySerializedAs("Transparent_mat")] public Material transparentMat;
        [FormerlySerializedAs("RockGreen_mat")] public Material rockGreenMat;
        [FormerlySerializedAs("RockDoor")] public Transform rockDoor;
        [FormerlySerializedAs("AudioWall")] public AudioSource audioWall;
        [FormerlySerializedAs("AudioReset")] public AudioSource audioReset;
        public GameObject[] cells;
        private bool _hasAudiowall;
        private bool _reset;

        private bool _open;
        private Vector3 _target1,_target2;

        private void Start()
        {
            var position = rockDoor.transform.position;
            _target1 = new Vector3(position.x, position.y, position.z + 3f);
            var position1 = transform.position;
            _target2 = new Vector3(position1.x, position1.y, position1.z + 3f);
            _hasAudiowall = audioWall != null;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                OpenDoor();
            }
            if (_open)
            {
                var step = 3f * Time.deltaTime; // calculate distance to move
                var position = rockDoor.transform.position;
                position = Vector3.MoveTowards(position, _target1, step);
                rockDoor.transform.position = position;
                transform.position = Vector3.MoveTowards(transform.position, _target2, step);
                var dist = Vector3.Distance(position, _target1);
                if (dist < 0.01f)
                {
                    Destroy(this);
                }
            }
            else
                ControlIfComplete();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("PlayerPath")) return;
            Reset();
            audioReset.Play();
            GetComponent<DrawLine>().ResetLine();
        }

        private void Reset()
        {
            for (var i = 0; i < cells.Length && !_open; i++) {
                cells[i].GetComponent<Collider>().isTrigger = true;
                cells[i].GetComponent<ChangeMaterialPath>().activate = false;
                cells[i].transform.GetChild(0).gameObject.GetComponent<Renderer>().material = transparentMat;
            }
        }

        private void ControlIfComplete()
        {
            var n = 0;
            foreach (var t in cells)
            {
                if (t.GetComponent<ChangeMaterialPath>().activate)
                    n++;
                else
                    break;
            }

            if (n != cells.Length) return;
            for (var i = 0; i < n; i++)
            {
                cells[i].transform.GetChild(0).gameObject.GetComponent<Renderer>().material = rockGreenMat;
                Destroy(cells[i].GetComponent<ChangeMaterialPath>());
            }
            OpenDoor();
        }

        void OpenDoor()
        {
            if(_hasAudiowall)
                audioWall.Play();
            _open = true;
        }
    }
}
