using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketManage : MonoBehaviour
{
    public GameObject Bucket;
    public GameObject GolfDoor;
    public Material TransparentGreen_mat;


    void OnTriggerEnter(Collider other)
    {
            Bucket.GetComponent<Renderer>().material = TransparentGreen_mat;
            GolfDoor.GetComponent<OpenGolfDoor>().AddBall();
            Destroy(other.gameObject);
            Destroy(gameObject);
    }
}
