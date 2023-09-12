using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNearInteractableObjectJoy : MonoBehaviour
{
    private GameObject ObjectPickUp;
    public bool objectTaken = false;
    public bool kinematicAfterPosition = false;

    public GameObject KeyInteractionImage;
    // Start is called before the first frame update
    void Start()
    {
        if (KeyInteractionImage != null)
            KeyInteractionImage.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interactable")
        {
            ObjectPickUp = other.gameObject;
            //SHOW KEY IMAGE
            if (KeyInteractionImage != null && !objectTaken)
                KeyInteractionImage.SetActive(true);

            //PUT OBJECT INTO STONE
            if (!ObjectPickUp.GetComponent<InteractableObject>().pickedUp)
            {
                //HIDE KEY IMAGES
                if (ObjectPickUp.GetComponent<InteractableObject>().KeyInteractionImage != null)
                {
                    ObjectPickUp.GetComponent<InteractableObject>().KeyInteractionImage.SetActive(false);
                    ObjectPickUp.GetComponent<InteractableObject>().KeyInteractionImage = null;
                }

                if (KeyInteractionImage != null) {
                    KeyInteractionImage.SetActive(false);
                    KeyInteractionImage =null;
                }

                //PUT OBJECT INTO PLACE
                ObjectPickUp.transform.position = transform.position;
                ObjectPickUp.GetComponent<Rigidbody>().isKinematic = kinematicAfterPosition;
                ObjectPickUp.transform.parent = transform;
                if (kinematicAfterPosition) //the object became small-> so you can't interact next time
                    ObjectPickUp.transform.localScale = Vector3.zero;
                objectTaken = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactable")
        {
            //HIDE KEY IMAGE
            if (KeyInteractionImage != null && !objectTaken)
                KeyInteractionImage.SetActive(false);
        }
    }
}
