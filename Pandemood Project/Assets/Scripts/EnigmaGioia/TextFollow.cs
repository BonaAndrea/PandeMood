using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFollow : MonoBehaviour
{
    public GameObject following;
    [Range(0.0f, 1.0f)]
    public float interested; //how interested the follower is in the thing it's following :)

    [Range(0.0f, 3.0f)]
    public float heightDistance;

    //THE TEXT FOLLOW THE OBJECT
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(following.transform.position.x, following.transform.position.y+heightDistance, following.transform.position.z), interested);
    }
}
