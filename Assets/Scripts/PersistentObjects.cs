using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    public static PersistentObjects Instance; 

    public GameObject customToy;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
