using System;
using System.Collections;
using System.Collections.Generic;

using Firebase.Extensions;
using Firebase.Storage;

using UnityEngine;

public class FirebaseStorageController : MonoBehaviour
{
    
    private FirebaseStorage _firebaseStorageInstance;
    private GameObject _imagePlaceholder;
    
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
        _firebaseStorageInstance = FirebaseStorage.DefaultInstance;
    }

    private void Start()
    {
        {
            _imagePlaceholder = GameObject.Find("Downloaded_Image");
            DownloadImageAsync("gs://cg-01-7bb16.appspot.com/Thumbnails/Image1.png");
        }
    }

    public void DownloadImageAsync(String url)
    {
        // Create a storage reference from our storage service
        StorageReference imageRef =
            _firebaseStorageInstance.GetReferenceFromUrl(url);
        
        // Download in memory with a maximum allowed size of 1MB (1 * 1024 * 1024 bytes)
        const long maxAllowedSize = 1 * 1024 * 1024;
        imageRef.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled) {
                Debug.LogException(task.Exception);
                // Uh-oh, an error occurred!
            }
            else {
                byte[] fileContents = task.Result;
                Debug.Log($"{imageRef.Name} finished downloading!");
                //Display the image inside _imagePlaceholder
            }
        });
    }
    
}
