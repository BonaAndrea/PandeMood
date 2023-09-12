using UnityEngine;
using UnityEngine.Serialization;

namespace Enigma_Tristezza
{
    public class BubbleLifebuoy : MonoBehaviour
    {
        public Transform target;
        [FormerlySerializedAs("TransparentGreen")] public Material transparentGreen;
        public float speed = 0.5f;

        [FormerlySerializedAs("LifeBuoy")] public GameObject lifeBuoy;
        private LightObject _lifeLight;
        // Start is called before the first frame update
        void Start()
        {
            _lifeLight = lifeBuoy.GetComponent<LightObject>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_lifeLight.illuminated)
            {
                var step = speed * Time.deltaTime; // calculate distance to move
                Transform transform1;
                var position = target.position;
                (transform1 = transform).position = Vector3.MoveTowards(transform.position, position, step);
                var dist = Vector3.Distance(transform1.position, position);
                if (dist < 0.01f)
                {
                    GetComponent<Renderer>().material = transparentGreen; //change material
                    if(target.gameObject.GetComponent<Shape>()!=null)
                        target.gameObject.GetComponent<Shape>().ShapeInsert(); //shape insert correctly
                    Destroy(this);
                }
            }
        }
    }
}
