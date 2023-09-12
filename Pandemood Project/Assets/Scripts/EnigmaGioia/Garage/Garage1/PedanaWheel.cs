using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedanaWheel : MonoBehaviour
{
    public GameObject GarageDoor;
    public Material TransparentGreen_mat;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter()
    {
        GetComponent<Renderer>().material = TransparentGreen_mat;
        GarageDoor.GetComponent<DoorGarageOpen>().AddOpen();
        Destroy(this);
    }
}
