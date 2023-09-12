using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedanaGolfMove : MonoBehaviour
{
    public GameObject MachineGolf;
    public bool clicked = false;

    public GameObject Point;

    public AudioSource AudioArmMachine;

    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (clicked)
        {
                MachineGolf.transform.position = Vector3.MoveTowards(MachineGolf.transform.position, Point.transform.position, speed * Time.deltaTime);
                if (Vector3.Distance(MachineGolf.transform.position, Point.transform.position) < 0.01f)
                {
                    if (AudioArmMachine != null)
                        AudioArmMachine.Stop();
                }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            clicked = true;
            if (AudioArmMachine != null)
                AudioArmMachine.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            clicked = false;
            if (AudioArmMachine != null)
                AudioArmMachine.Stop();
        }
    }
}
