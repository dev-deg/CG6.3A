using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

using Firebase.Extensions;
using Firebase.Storage;

using UnityEngine;
using UnityEngine.UI;

public class FirebaseStorageController : MonoBehaviour
{
    
    private FirebaseStorage _firebaseStorageInstance;
    [SerializeField] private GameObject RawImagePrefab;
    private GameObject _thumbnailContainer;
    private List<GameObject> _rawImageList;
    private List<AssetData> _assetData;
    public enum DownloadType
    {
        Thumbnail, Manifest
    }
    
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
        _thumbnailContainer = GameObject.Find("Thumbnails_Container");
        _rawImageList = new List<GameObject>();
        //Download Manifest
        DownloadFileAsync("gs://cg-01-7bb16.appspot.com/manifest.xml",DownloadType.Manifest);
    }

    public void DownloadFileAsync(String url, DownloadType dType)
    {
        // Create a storage reference from our storage service
        StorageReference imageRef =
            _firebaseStorageInstance.GetReferenceFromUrl(url);

        // Download in memory with a maximum allowed size of 1MB (1 * 1024 * 1024 bytes)
        const long maxAllowedSize = 1 * 1024 * 1024;
        imageRef.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogException(task.Exception);
                // Uh-oh, an error occurred!
            }
            else
            {
                byte[] fileContents = task.Result;
                Debug.Log($"{imageRef.Name} finished downloading!");
                if (dType == DownloadType.Thumbnail)
                {
                    //Load Image
                    StartCoroutine(LoadImage(fileContents));
                }
                else if (dType == DownloadType.Manifest)
                {
                    //Load the manifest
                    StartCoroutine(LoadManifest(fileContents));
                }

            }
        });
    }

    IEnumerator LoadManifest(byte[] fileContents)
        {
            //Converting from byte array to String UTF8
            XDocument manifest = XDocument.Parse(System.Text.Encoding.UTF8.GetString(fileContents));
            _assetData = new List<AssetData>();
            foreach (XElement element in manifest.Root.Elements())
            {
                string idStr = element.Element("id")?.Value;
                int id = (idStr != null) ? int.Parse(idStr) : 0;
                string nameStr = element.Element("name")?.Value;
                string urlStr = element.Element("thumbnail")?.Element("url")?.Value;
                string priceStr = element.Element("price")?.Element("value")?.Value;
                float price = (priceStr != null) ? float.Parse(priceStr) : 0f;
                string currencyStr = element.Element("price")?.Element("currency")?.Value;
                AssetData.CURRENCY currency;
                switch (currencyStr)
                {
                    case "diamonds":
                        currency = AssetData.CURRENCY.Diamonds;
                        break;
                    case "gold":
                        currency = AssetData.CURRENCY.Gold;
                        break;
                    default:
                        currency = AssetData.CURRENCY.Default;
                        break;
                }

                AssetData newAsset = new AssetData(id, nameStr, urlStr, price, currency);
                
               
                Debug.Log(newAsset.ToString());
                _assetData.Add(newAsset);
                DownloadFileAsync(newAsset.ThumbnailUrl,DownloadType.Thumbnail);
            }
            yield return null;
        }    

    IEnumerator LoadImage(byte[] fileContents)
    {
        // Display the image inside _imagePlaceholder
        GameObject rawImage = Instantiate(RawImagePrefab, _thumbnailContainer.transform.position, Quaternion.identity, _thumbnailContainer.transform);
        rawImage.name = "DownloadedImage_" + _rawImageList.Count;
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(fileContents);
        rawImage.GetComponent<RawImage>().texture = tex;
        _rawImageList.Add((rawImage));
        yield return null;
    }
}
