using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedanaOpenDoor : MonoBehaviour
{
    public GameObject Door;

    void OnTriggerEnter()
    {
        Door.GetComponent<DoorGarageOpen>().AddOpen();
    }
}
