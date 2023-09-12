using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    public GameObject WheelToInsert;

    private bool insert = false;
    public AudioSource AudioAddOpen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, WheelToInsert.transform.position) < 0.05f && !insert)
        {
            insert = true;
            WheelToInsert.transform.position = transform.position;
            WheelToInsert.transform.parent = transform.parent;
            if (AudioAddOpen != null)
                AudioAddOpen.Play();
            Destroy(gameObject);
        }
    }
}
