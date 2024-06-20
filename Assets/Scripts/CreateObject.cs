using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateObject : MonoBehaviour
{
    public GameObject sampleobject;
    public Transform pos;

    public void AddObject()
    {
        Vector3 spawnPos = pos.forward * 3 + pos.position;
        Instantiate(sampleobject, spawnPos, pos.rotation);
    }

    public void AddCustomToy()
    {
        Vector3 spawnPos = pos.forward * 3 + pos.position;
        var toCreate = sampleobject;
        if (PersistentObjects.Instance.customToy != null)
        {
            toCreate = PersistentObjects.Instance.customToy;
        }
        GameObject customToy = Instantiate(toCreate, spawnPos, toCreate.transform.rotation);
        customToy.AddComponent<Rigidbody>();
        
        customToy.transform.localScale = new Vector3(customToy.transform.localScale.x * 0.2f, 
                                                     customToy.transform.localScale.y * 0.2f, 
                                                     customToy.transform.localScale.z * 0.2f);
    }

    
}
