using UnityEngine;
using UnityEngine.Serialization;

namespace Enigma_Tristezza
{
    public class ChangeMaterialPath : MonoBehaviour
    {
        [FormerlySerializedAs("Transparent_mat")] public Material transparentMat;
        [FormerlySerializedAs("TransparentRed_mat")] public Material transparentRedMat;
        [FormerlySerializedAs("TransparentGreen_mat")] public Material transparentGreenMat;
        [FormerlySerializedAs("AudioError")] public AudioSource audioError;
        public bool activate = false;

        private GameObject _rock;
    

        // Start is called before the first frame update
        private void Start()
        {
            _rock = gameObject.transform.GetChild(0).gameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("PlayerPath")) return;
            ChangeMaterialTo(transparentGreenMat, transparentRedMat, false);
            ChangeMaterialTo(transparentMat, transparentGreenMat, true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("PlayerPath"))
            {
                //ChangeMaterialTo(TransparentGreen_mat, GreenRock_mat);
                //GetComponent<Collider>().isTrigger = false;
            }
        }

        // creates a new material instance that looks like the old material
        private void ChangeMaterialTo(Object matToControl, Material matChange, bool active)
        {
            var mat1 = _rock.GetComponent<Renderer>().material.name;
            var mat2 = matToControl.name;
            if (!mat1.Contains(mat2)) return;
            _rock.GetComponent<Renderer>().material = matChange;
            activate = active;
            var drawLine = transform.parent.gameObject.GetComponent<DrawLine>();
            if (activate)
            {
                //DRAW LINE
                if (drawLine != null)
                {
                    drawLine.SetNewPoint(transform);
                }
            }
            else
                //DELETE LINE
            if (drawLine != null){
                audioError.Play();
                drawLine.ResetLine();
            }
        }
    }
}
