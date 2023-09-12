using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMechanicArm : MonoBehaviour
{
    public GameObject MechanicArm;
    public Transform TargetPoint;
    public float speed = 2f;

    public AudioSource AudioArmMachine;

    private bool move = false;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = MechanicArm.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            MechanicArm.transform.position = Vector3.MoveTowards(MechanicArm.transform.position, TargetPoint.position, speed * Time.deltaTime);
            if (Vector3.Distance(MechanicArm.transform.position, TargetPoint.position) < 0.01f)
            {
                if (AudioArmMachine != null)
                    AudioArmMachine.Stop();
            }
        }
        else
        {
            MechanicArm.transform.position = Vector3.MoveTowards(MechanicArm.transform.position, startPosition, speed * Time.deltaTime);
            if (Vector3.Distance(MechanicArm.transform.position, startPosition) < 0.01f)
            {
                if (AudioArmMachine != null)
                    AudioArmMachine.Stop();
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
             move = true;
             if (AudioArmMachine != null)
                 AudioArmMachine.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            move = false;
            if (AudioArmMachine != null)
                AudioArmMachine.Play();
        }
    }
}
