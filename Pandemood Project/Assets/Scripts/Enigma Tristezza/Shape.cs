using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public int NumberShapes;
    public bool completed=false;
    public GameObject ShapeDoor;
    public AudioSource AudioWall;
    public AudioSource AudioComplete;
    public Material TransparentGreen;
    private int n=0;

    private bool open = false;
    private Vector3 target;

    void Start()
    {
        target = new Vector3(ShapeDoor.transform.position.x, ShapeDoor.transform.position.y, ShapeDoor.transform.position.z + 3f);
    }

    void Update()
    {
        if (open)
        {
            float step = 3f * Time.deltaTime; // calculate distance to move
            ShapeDoor.transform.position = Vector3.MoveTowards(ShapeDoor.transform.position, target, step);
            float dist = Vector3.Distance(ShapeDoor.transform.position, target);
            if (dist < 0.01f)
            {
                Destroy(this);
            }
        }
    }

    public void ShapeInsert()
    {
        n++;
        if (n == NumberShapes)
        {
            completed = true;
            GetComponent<Renderer>().material = TransparentGreen;
            if(AudioWall!=null)
                AudioWall.Play();
            if (AudioComplete != null)
                AudioComplete.Play();
            open = true;
        }
    }
}
