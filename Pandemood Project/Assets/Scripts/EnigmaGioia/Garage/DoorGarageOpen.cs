using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGarageOpen : MonoBehaviour
{
    public int NToOpen;
    public Transform TargetOpen;
    public float speedOpen = 5f;
    private bool open = false;
    private int n = 0;

    public AudioSource AudioCompleted;
    public AudioSource AudioAddOpen;

    void Start()
    {
    }

    void Update()
    {
        if (open && Vector3.Distance(transform.position, TargetOpen.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetOpen.position, speedOpen * Time.deltaTime);
        }
    }

    public void AddOpen()
    {
        n++;
        if (n == NToOpen) //open
        {
            open = true;
            if (AudioCompleted != null)
                AudioCompleted.Play();
        }

        if(n < NToOpen )
            if (AudioAddOpen != null)
                AudioAddOpen.Play();
    }
}
