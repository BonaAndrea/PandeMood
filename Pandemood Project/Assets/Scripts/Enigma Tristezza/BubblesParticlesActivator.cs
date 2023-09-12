using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesParticlesActivator : MonoBehaviour
{
    public ParticleSystem bubbleParticles;
    public GameObject WaterSurface;

    private float waterHeight;
    private bool isUnderwater;
    // Start is called before the first frame update
    void Start()
    {
        bubbleParticles.Stop();
        waterHeight = WaterSurface.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.y < (waterHeight-1f)) != isUnderwater)
        {
            isUnderwater = transform.position.y < waterHeight;
            if (isUnderwater) bubbleParticles.Play();
            if (!isUnderwater) bubbleParticles.Stop();
        }
    }
}
