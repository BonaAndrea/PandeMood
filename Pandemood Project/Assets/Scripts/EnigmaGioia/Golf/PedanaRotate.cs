using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedanaRotate : MonoBehaviour
{
    public GameObject PareteGolf;
    public bool rotate = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            rotate = true;
            PareteGolf.transform.rotation = Quaternion.Euler(0, 0, PareteGolf.transform.eulerAngles.z + 90);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            rotate = false;
            PareteGolf.transform.rotation = Quaternion.Euler(0, 0, PareteGolf.transform.eulerAngles.z - 90);
        }
    }
}
