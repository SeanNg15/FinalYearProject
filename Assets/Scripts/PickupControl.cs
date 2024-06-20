using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupControl : MonoBehaviour
{
    public Transform holdArea;
    public Transform objectHolder;

    public GameObject childObject;
    ChildMood childMood;

    GameObject heldObject;
    Rigidbody heldObjectRb;

    private static int[] angles = {0, 90, 180, 270};
    private int ang = 0;

    public float pickupRange;
    public float pickupForce;

    public float updown;
    public float upperLimit;
    public float lowerLimit;

    void Start()
    {
        childMood = childObject.GetComponent<ChildMood>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerCam.paused) 
        {
            return;
        }

        //interacting with child
        //raycast to what is seen
        bool onSight = Physics.Raycast(objectHolder.position, transform.TransformDirection(Vector3.forward), out RaycastHit seen, 2);
        if (onSight && seen.transform.gameObject.CompareTag("child"))
        {
            childMood.interacted = true;
        } 
        else
        {
            childMood.interacted = false;
        }

        //interacting with objects
        if (Input.GetMouseButtonDown(0)) 
        {
            if (heldObject == null) 
            {
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, pickupRange))
                {
                    PickupObject(hit.transform.gameObject);
                }
            } 
            else 
            {
                DropObject();
            } 
        }

        if (heldObject != null)
        {
            MoveObject();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateObjectHori();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Destroy(heldObject);
            }
            if (Input.mouseScrollDelta.y != 0)
            {
                MoveUpOrDown(Input.mouseScrollDelta.y);
            }
        }
        
    }

    void MoveUpOrDown(float delta)
    {
        Vector3 oldPos = heldObject.transform.position;
        if (delta > 0)
        {
            float newY = heldObject.transform.position.y + updown;
            heldObject.transform.position = new Vector3(oldPos.x, Mathf.Min(newY, transform.position.y + upperLimit), oldPos.z);
        } 
        else 
        {
            float newY = heldObject.transform.position.y - updown;
            heldObject.transform.position = new Vector3(oldPos.x, Mathf.Max(newY, transform.position.y - lowerLimit), oldPos.z);
        }
    }


    void RotateObjectHori()
    {
        heldObjectRb.transform.rotation = Quaternion.Euler(0, angles[ang], 0);
        ang += 1;
        if (ang >= angles.Length) 
        {
            ang = 0;
        }

    }

    void PickupObject(GameObject pickedUp)
    {
        if (pickedUp.GetComponent<Rigidbody>() && !pickedUp.CompareTag("child"))
        {
            heldObjectRb = pickedUp.GetComponent<Rigidbody>();
            heldObjectRb.useGravity = false;
            heldObjectRb.drag = 10;
            heldObjectRb.constraints = RigidbodyConstraints.FreezeRotation;
            heldObjectRb.transform.rotation = Quaternion.identity;
            heldObjectRb.transform.parent = objectHolder;

            heldObject = pickedUp;
        }
    }

    void DropObject()
    {
        
        if (heldObject.CompareTag("wall decor"))
        {
            heldObjectRb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            heldObjectRb.useGravity = true;
            heldObjectRb.drag = 1;
            heldObjectRb.constraints = RigidbodyConstraints.None;
        }

        heldObjectRb.transform.parent = null;

        heldObject = null;

    }

    void MoveObject()
    {
        float dist = heldObject.GetComponent<Collider>().bounds.size.magnitude;
        if (Vector3.Distance(heldObject.transform.position, holdArea.position) > Math.Max(dist + 0.3f, pickupRange))
        {
            Vector3 moveDirection = holdArea.position - heldObject.transform.position;
            heldObjectRb.AddForce(moveDirection * pickupForce);
            //clamp max speed
            heldObjectRb.velocity = Vector3.ClampMagnitude(heldObjectRb.velocity, 5);
        }
        
    }
}
