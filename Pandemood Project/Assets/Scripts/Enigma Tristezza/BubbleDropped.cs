using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleDropped : MonoBehaviour
{
    public float speed = 1f;
    public AudioSource AudioPopBubble;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime* speed, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ExplodeBubble" || other.tag == "Player")
        {
            if(AudioPopBubble!=null)
                AudioPopBubble.Play();
            Destroy(gameObject);
        }
    }
}
