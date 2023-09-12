using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDirection : MonoBehaviour
{
    public delegate void DirectionChanged(string corner);
    public static event DirectionChanged OnDirectionChanged;
    public bool triggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Corner") || collision.gameObject.tag.Equals("Corner1")
            || collision.gameObject.tag.Equals("Corner2") || collision.gameObject.tag.Equals("Corner3"))
        {
            triggered = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((collision.gameObject.tag.Equals("Corner") || collision.gameObject.tag.Equals("Corner1")
            || collision.gameObject.tag.Equals("Corner2") || collision.gameObject.tag.Equals("Corner3"))
            && Input.GetAxis("Vertical") >= 0.9f && triggered) {
            Debug.Log("Triggered");
            if (OnDirectionChanged != null)
                triggered = false;
                OnDirectionChanged(collision.gameObject.tag);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Corner") || collision.gameObject.tag.Equals("Corner1")
            || collision.gameObject.tag.Equals("Corner2") || collision.gameObject.tag.Equals("Corner3"))
        {
            triggered = false;
        }
    }
}
