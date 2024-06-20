using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticky : MonoBehaviour
{
    void OnCollisionEnter(Collision surface)
    {
        if (surface.gameObject.tag == "wall")
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            print("HIT");
        }
    }

    void OnCollisionExit(Collision surface)
    {
        if (surface.gameObject.tag == "wall")
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            print("LEFT");
        }
    }
}
