using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enigma_Tristezza
{
    public class DropBubbleTube : MonoBehaviour
    {
        [FormerlySerializedAs("Bubble")] public GameObject bubble;
        public float timeDropBubble = 5f;

        private GameObject _player;
        // Start is called before the first frame update
        private void Start()
        {
            StartCoroutine(DropBubble());
            _player=GameObject.FindWithTag("Player");
        }

        private void FixedUpdate()
        {
            bubble.transform.position = transform.position;
        }

        private IEnumerator DropBubble()
        {
            yield return new WaitForSeconds(timeDropBubble);
            if(Vector3.Distance(_player.transform.position, transform.position) < 30f) { //START BUBBLING ONLY IF PLAYER NEAR
                GameObject instantiate = Instantiate(this.bubble);
                instantiate.transform.position = transform.position;
                RallentyTime rallentyTime = instantiate.GetComponent<RallentyTime>();
                instantiate.GetComponent<BubbleDropped>().speed = 1f;
                if (!(rallentyTime is null)) //start in rallenty if needed
                    if (rallentyTime.rallenty)
                        rallentyTime.rallenty = true;
            }
            StartCoroutine(DropBubble());
        }
    }
}
