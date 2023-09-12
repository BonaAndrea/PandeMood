using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPerlDoor : MonoBehaviour
{
    public GameObject PerlDoor;
    public AudioSource AudioWall;
    public AudioSource AudioComplete;

    private bool open=false;
    private Vector3 target;

    void Start()
    {
        target= new Vector3(PerlDoor.transform.position.x, PerlDoor.transform.position.y, PerlDoor.transform.position.z + 3f);
    }
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<GetNearInteractableObjectSad>().objectTaken)
        {
            GetComponent<GetNearInteractableObjectSad>().objectTaken = false;
            if (AudioWall != null)
                AudioWall.Play();
            if (AudioComplete != null)
                AudioComplete.Play();
            open = true;
        }

        if (open)
        {
            float step = 3f * Time.deltaTime; // calculate distance to move
            PerlDoor.transform.position = Vector3.MoveTowards(PerlDoor.transform.position, target, step);
            float dist = Vector3.Distance(PerlDoor.transform.position, target);
            if (dist < 0.01f)
            {
                Destroy(this);
            }
        }

    }
}
