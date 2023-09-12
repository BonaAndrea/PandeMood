using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTargetHub : MonoBehaviour
{
    //Bezier Fields
    [SerializeField]
    private Transform[] routes;

    private int routeToGo;

    public float tParam = 1f;

    private Vector3 cameraPosition;

    public float speedModifier;

    private bool coroutineAllowed;

    public Transform target;
    [Range(1f, 40f)] public float laziness = 10f;
    public bool lookAtTarget = true;
    public bool takeOffsetFromInitialPos = true;
    public Vector3 fromInitialPositionOffset;
    public Vector3 generalOffset;
    Vector3 whereCameraShouldBe;
    bool warningAlreadyShown = false;

    public float rotationStep = 2f;
    public bool isZFreezed = false;
    private float rotationCount = 0f;
    private bool turning = false;
    private bool isRotatingBackwards = false;


    private void Start()
    {
        if (takeOffsetFromInitialPos && target != null) generalOffset = transform.position - target.position - fromInitialPositionOffset;
        ChangeDirection.OnDirectionChanged += TurnOf90Degrees;

        //Bezier Follow Target
        routeToGo = 0;
        tParam = 0f;
        coroutineAllowed = true;
    }
    void TurnOf90Degrees(string corner)
    {
        switch (corner)
        {
            case "Corner":
                routeToGo = 0;
                isZFreezed = !isZFreezed;
                if (isZFreezed)
                {
                    tParam = 1;
                    isRotatingBackwards = true;
                }
                else
                {
                    tParam = 0;
                    isRotatingBackwards = false;
                }
                break;
            case "Corner1":
                routeToGo = 1;
                isZFreezed = !isZFreezed;
                if (isZFreezed)
                {
                    tParam = 0;
                    isRotatingBackwards = false;
                }
                else
                {
                    tParam = 1;
                    isRotatingBackwards = true;
                }
                break;
            case "Corner2":
                routeToGo = 2;
                isZFreezed = !isZFreezed;
                if (isZFreezed)
                {
                    tParam = 1;
                    isRotatingBackwards = true;
                }
                else
                {
                    tParam = 0;
                    isRotatingBackwards = false;
                }
                break;
            case "Corner3":
                routeToGo = 3;
                isZFreezed = !isZFreezed;
                if (isZFreezed)
                {
                    tParam = 0;
                    isRotatingBackwards = false;
                }
                else
                {
                    tParam = 1;
                    isRotatingBackwards = true;
                }
                break;
            default:
                Debug.Log("String invalid");
                break;
        }
        turning = true;
        StartCoroutine(GoByTheRoute(routeToGo));
        Debug.Log("Camera Girata!");
    }

    void FixedUpdate()
    {
        if (target != null && !turning)
        {
            whereCameraShouldBe = target.position + generalOffset;
            transform.position = Vector3.Lerp(transform.position, whereCameraShouldBe, 1 / laziness);

            if (lookAtTarget) transform.LookAt(target);
        }
        else if (target == null)
        {
            if (!warningAlreadyShown)
            {
                Debug.Log("Warning: You should specify a target in the simpleCamFollow script.", gameObject);
                warningAlreadyShown = true;
            }
        }
    }

    private IEnumerator GoByTheRoute(int routeNumber)
    {
        Vector3 p0 = routes[routeNumber].GetChild(0).position;
        Vector3 p1 = routes[routeNumber].GetChild(1).position;
        Vector3 p2 = routes[routeNumber].GetChild(2).position;
        Vector3 p3 = routes[routeNumber].GetChild(3).position;

        if (tParam == 1)
        {
            while (tParam > 0)
            {
                tParam -= Time.deltaTime * speedModifier;

                cameraPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                    3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                    3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                    Mathf.Pow(tParam, 3) * p3;

                transform.position = cameraPosition;
                transform.LookAt(target);
                yield return new WaitForEndOfFrame();
            }
        }
        else if (tParam == 0)
        {
            while (tParam < 1)
            {
                tParam += Time.deltaTime * speedModifier;

                cameraPosition = Mathf.Pow(1 - tParam, 3) * p0 +
                    3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
                    3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
                    Mathf.Pow(tParam, 3) * p3;

                transform.position = cameraPosition;
                transform.LookAt(target);
                yield return new WaitForEndOfFrame();
            }
        }

        generalOffset = transform.position - target.position - fromInitialPositionOffset;
        Debug.Log("Routine Terminated");
        turning = false;
    }
}
