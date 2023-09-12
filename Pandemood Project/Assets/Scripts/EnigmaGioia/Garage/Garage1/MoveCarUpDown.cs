using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCarUpDown : MonoBehaviour
{
    public GameObject Car;
    public Transform TargetPoint;
    public float speed = 2f;
    public AudioSource AudioArmMachine;

    private bool move = false;
    private Vector3 startPosition;
    private bool waitTime = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = Car.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            Car.transform.position = Vector3.MoveTowards(Car.transform.position, TargetPoint.transform.position, speed * Time.deltaTime);
            if (Vector3.Distance(Car.transform.position, TargetPoint.position) < 0.01f)
            {
                if (AudioArmMachine != null)
                    AudioArmMachine.Stop();
            }
        }
        else
        {
            Car.transform.position = Vector3.MoveTowards(Car.transform.position, startPosition, speed * Time.deltaTime);
            if (Vector3.Distance(Car.transform.position, startPosition) < 0.01f)
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
            if (!move && !waitTime)
            {
                move = true;
                StartCoroutine(WaitTriggerTime());
            }
            if (move && !waitTime)
            {
                move = false;
                StartCoroutine(WaitTriggerTime());
            }

            if (AudioArmMachine != null)
                AudioArmMachine.Play();
        }
    }

    private IEnumerator WaitTriggerTime()
    {
        waitTime= true;
        yield return new WaitForSeconds(0.5f);
        waitTime = false;
    }
}
