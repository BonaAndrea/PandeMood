using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCarManage : MonoBehaviour
{
    public GameObject DoorGarage;
    public GameObject PlatformEnigma;
    public GameObject DestinationMiniCar;
    public Material TransparentGreen_mat;
    public AudioSource AudioError;
    private Vector3 StartMiniCarPosition;

    // Start is called before the first frame update
    void Start()
    {
        StartMiniCarPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, DestinationMiniCar.transform.position) < 0.01f)
        {
            Completed();
        }
    }

    void OnTriggerEnter()
    {
        //se si scontra contro il blocco ritorna alla posizione di partenza (ricomincia da capo)
        transform.position = StartMiniCarPosition;
        if (AudioError != null)
            AudioError.Play();
    }

    void Completed()
    {
        for (int i = 0; i < DestinationMiniCar.transform.childCount; i++) //coloro di verde tutti i componenti dell'auto
        {
            GameObject element_of_car = DestinationMiniCar.transform.GetChild(i).gameObject;
            if (element_of_car.GetComponent<Renderer>() != null)
                element_of_car.GetComponent<Renderer>().material= TransparentGreen_mat;
        }
        DoorGarage.GetComponent<DoorGarageOpen>().AddOpen(); //consente apertura porta
        PlatformEnigma.GetComponent<DoorGarageOpen>().AddOpen();  //piattaforma si sposta
        Destroy(this);
    }
}
