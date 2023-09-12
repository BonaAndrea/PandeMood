using UnityEngine;

namespace Blocks.MovingPlatform
{
    public class PlayerParenting : MonoBehaviour
    {
        // Start is called before the first frame update
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.transform.SetParent(transform);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.transform.SetParent(null);
            }
        }
    }
}
