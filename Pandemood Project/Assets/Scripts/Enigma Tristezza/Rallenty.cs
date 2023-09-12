using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rallenty : MonoBehaviour
{
    public bool rallenty = false;
    public AudioSource AudioRallentyStart;
    public AudioSource AudioRallentyAmbience;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //RALLENTY
        if (Input.GetMouseButtonDown(1))
            if (!rallenty)
            { //RALLENTY
                rallenty = true;
                AudioRallentyStart.Play();
                AudioRallentyAmbience.Play();
            }
            else
            { //NO RALLENTY
                rallenty = false;
                AudioRallentyStart.Stop();
                AudioRallentyAmbience.Stop();
            }
    }
}
