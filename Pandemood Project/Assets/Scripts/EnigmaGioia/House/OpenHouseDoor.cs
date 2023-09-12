using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenHouseDoor : MonoBehaviour
{
    public GameObject HouseDoor;
    public AudioSource AudioComplete;
    public Material TransparentGreen_mat;

    private bool open = false;
    private Vector3 target;
    private GetNearInteractableObjectJoy nearObject;

    void Start()
    {
        target = new Vector3(HouseDoor.transform.position.x, HouseDoor.transform.position.y, HouseDoor.transform.position.z + 3f);
        nearObject = GetComponent<GetNearInteractableObjectJoy>();
    }
    // Update is called once per frame
    void Update()
    {
        if (nearObject.objectTaken)
        {
            nearObject.objectTaken = false;
            if (AudioComplete != null)
                AudioComplete.Play();
            open = true;
            transform.GetChild(0).gameObject.GetComponent<Renderer>().material = TransparentGreen_mat;
        }

        if (open)
        {
            float step = 3f * Time.deltaTime; // calculate distance to move
            HouseDoor.transform.position = Vector3.MoveTowards(HouseDoor.transform.position, target, step);
            float dist = Vector3.Distance(HouseDoor.transform.position, target);
            if (dist < 0.01f)
            {
                Destroy(this);
            }
        }

    }
}
