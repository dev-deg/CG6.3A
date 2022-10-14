using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseStorageController : MonoBehaviour
{
    //Singleton
    public static FirebaseStorageController Instance
    {
        get;
        private set;
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
    }
}
