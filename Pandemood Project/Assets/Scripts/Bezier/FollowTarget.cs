using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{

    public Vector3 offset;

    public GameObject target;

    private Transform targetTransform;
    // Start is called before the first frame update
    void Start()
    {
        targetTransform = target.transform;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = targetTransform.position + offset;
    }
}
