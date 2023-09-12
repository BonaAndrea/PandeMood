using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public GameObject myHands; //reference to your hands/the position where you want your object to go
    public float distanceInteraction;
    public AudioSource AudioPickup;
    bool canpickup; //a bool to see if you can or cant pick up the item
    GameObject ObjectIwantToPickUp; // the gameobject closest to pickup
    GameObject ObjectPickUp; //the gameobject pick up
    bool hasItem; // a bool to see if you have an item in your hand

    GameObject[] InteractableObjects;


    // Start is called before the first frame update
    void Start()
    {
        canpickup = false;    //setting both to false
        hasItem = false;
        InteractableObjects = GameObject.FindGameObjectsWithTag("Interactable");  //find all objects interactable
    }

    // Update is called once per frame
    void Update()
    {

        //---------------RELEASE OBJECT--------------------
        if (Input.GetButtonDown("Interact") && hasItem)
        {  // can be e or any key
            ObjectPickUp.GetComponent<Rigidbody>().isKinematic = false; // make the rigidbody work again
            ObjectPickUp.transform.parent = null; // make the object no be a child of the hands
            ObjectPickUp.GetComponent<Collider>().enabled = true;
            ObjectPickUp.GetComponent<Collider>().isTrigger = false;
            hasItem = false;
            if (ObjectPickUp.GetComponent<InteractableObject>())
                ObjectPickUp.GetComponent<InteractableObject>().pickedUp = false; //object release
            AudioPickup.Play();
        }
        else
            if (canpickup == true) // if you enter the collider of the objecct
                {
                    //----------------PICKUP OBJECT-------------------
                    if (Input.GetButtonDown("Interact"))  // can be e or any key
                        if (hasItem == false) {
                            ObjectPickUp = ObjectIwantToPickUp;
                            ObjectPickUp.GetComponent<Rigidbody>().isKinematic = true;   //makes the rigidbody not be acted upon by forces
                            ObjectPickUp.transform.position = myHands.transform.position; // sets the position of the object to your hand position
                            ObjectPickUp.transform.parent = myHands.transform; //makes the object become a child of the parent so that it moves with the hands
                            ObjectPickUp.GetComponent<Collider>().isTrigger = true;
                            hasItem = true;
                            if(ObjectPickUp.GetComponent<InteractableObject>())
                                ObjectPickUp.GetComponent<InteractableObject>().pickedUp = true; //object picked up
                            AudioPickup.Play();
                        }
                }
    }

    void FixedUpdate()
    {
        //--------- CONSIDER THE CLOSEST OBJECT CAN PICK UP ----------
        if (InteractableObjects!=null)
           ControlDistanceFromObject();
    }


    //-----------SET CLOSEST OBJECT CAN PICK UP--------------
    private void ControlDistanceFromObject()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;

        foreach (GameObject obj in InteractableObjects)
        {
            if (!obj.GetComponent<InteractableObject>().cantPickUp)
            {
                float curDistance = Vector3.Distance(obj.transform.position, transform.position);
                if (curDistance < distance && curDistance < distanceInteraction)
                {
                    closest = obj;
                    distance = curDistance;
                }
            }
        }

        if (closest != null)
        {
            ObjectIwantToPickUp = closest;
            canpickup = true;
        }
        else
            canpickup = false;
    }
}
