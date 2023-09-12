using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCarToDestination : MonoBehaviour
{
    public GameObject MiniCar;
    public Transform TargetPoint;
    public float speed = 2f;

    public AudioSource AudioMiniCar;
    private bool move = false;

    // Update is called once per frame
    void Update()
    {
        if (move && Vector3.Distance(MiniCar.transform.position, TargetPoint.position) > 0.01f)
            MiniCar.transform.position = Vector3.MoveTowards(MiniCar.transform.position, TargetPoint.position, speed * Time.deltaTime);
        else
            if (AudioMiniCar != null)
                AudioMiniCar.Stop();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            move = true;
            if (AudioMiniCar != null)
                AudioMiniCar.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            move = false;
            if (AudioMiniCar != null)
                AudioMiniCar.Stop();
        }
    }
}
