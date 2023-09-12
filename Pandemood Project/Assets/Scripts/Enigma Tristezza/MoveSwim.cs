using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSwim : MonoBehaviour
{

 // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        this.transform.Translate(-x*0.1f, y*0.1f, 0);

    }
}
