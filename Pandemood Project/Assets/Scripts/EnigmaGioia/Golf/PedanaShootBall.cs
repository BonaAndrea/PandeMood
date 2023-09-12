using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedanaShootBall : MonoBehaviour
{
    public GameObject Ball;
    public GameObject StartPositionShot;
    public float impulseForce = 50f;
    public bool shot = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            shot = true;
            GameObject clone = Instantiate(Ball, StartPositionShot.transform.position, StartPositionShot.transform.rotation);
            clone.GetComponent<Rigidbody>().isKinematic = false;
            clone.GetComponent<Rigidbody>().AddForce(StartPositionShot.transform.forward *impulseForce);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            shot = false;
        }
    }
}
