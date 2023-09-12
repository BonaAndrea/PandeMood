using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedanaPress : MonoBehaviour
{
    public float heightPress=0.05f;
    public AudioSource AudioPedana;

    void OnTriggerEnter()
    {
            transform.position = new Vector3(transform.position.x, transform.position.y - heightPress, transform.position.z);
        if (AudioPedana != null)
            AudioPedana.Play();
    }

    void OnTriggerExit()
    {
            transform.position = new Vector3(transform.position.x, transform.position.y + heightPress, transform.position.z);
        if (AudioPedana != null)
            AudioPedana.Play();
    }
}
