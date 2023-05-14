// Import necessary namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

// Define serializable classes to deserialize JSON data
[System.Serializable]
public class ImageData
{
    public string url;
    public int width;
    public int height;
}

[System.Serializable]
public class ImageList
{
    public List<ImageData> XRL_below;
    public List<ImageData> XRL_head;
    public List<ImageData> XRL_left;
    public List<ImageData> XRL_main;
    public List<ImageData> XRL_right;

    public ImageList(List<ImageData> below, List<ImageData> head, List<ImageData> left, List<ImageData> main, List<ImageData> right)
    {
        XRL_below = below ?? new();
        XRL_head = head ?? new();
        XRL_left = left ?? new();
        XRL_main = main ?? new();
        XRL_right = right ?? new();
    }
}

// The main class to load images from a JSON file and create planes for each image
public class XRLManager : MonoBehaviour
{
    // Public variables to be set in the inspector
    public string serverUrl;
    public float marginMeters = 0.05f;
    public float pixelsPerMillimeter = 1f;
    public Vector2 maxDimsMeters = new(0.8f, 0.8f);
    public Texture2D errorTexture;

    private Quaternion defaultRotation = Quaternion.Euler(90f, 180f, 0f);

    public async void LoadPage(string url) {
        Debug.Log("Getting: "+url);
        // Loop through all children of the current object
        foreach (Transform child in transform) Destroy(child.gameObject);
        try
        {
            using (HttpClient client = new())
            {
                // Get the JSON data from the specified URL as an ImageList
                ImageList imageList = JsonUtility.FromJson<ImageList>(await client.GetStringAsync(serverUrl+"render?url="+url));

                // Create planes for each image list in the ImageList
                Vector2 mainOffsets = CreatePlanesForImageList(imageList.XRL_main, Vector2.zero);
                CreatePlanesForImageList(imageList.XRL_right, new(mainOffsets.x, 0));
                CreatePlanesForImageList(imageList.XRL_left, new(-mainOffsets.x, 0));
                CreatePlanesForImageList(imageList.XRL_head, new(0, mainOffsets.y));
                CreatePlanesForImageList(imageList.XRL_below, new(0, -mainOffsets.y));
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            // Create a new plane object and set its parent to the current transform
            GameObject errorPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            errorPlane.transform.parent = transform;

            // Set the plane's local position, local scale, and rotation in a single line
            errorPlane.transform.localRotation = defaultRotation;
            errorPlane.transform.localScale = Vector3.one * 0.1f;
            errorPlane.transform.localPosition = Vector3.zero;

            // Set the plane's texture to the errorTexture
            errorPlane.GetComponent<Renderer>().material.mainTexture = errorTexture;
        }
    }
    
    private Vector2 CreatePlanesForImageList(List<ImageData> imageList, Vector2 initialOffset)
    {
        Vector2? ret = null;

        foreach (ImageData imageData in imageList)
        {
            Vector2 dimMeters = new(imageData.width / 1000f / pixelsPerMillimeter, imageData.height / 1000f / pixelsPerMillimeter);

            Vector2 adjustmentRatio = new(dimMeters.x > maxDimsMeters.x ? maxDimsMeters.x / dimMeters.x : 1,
                                          dimMeters.y > maxDimsMeters.y ? maxDimsMeters.y / dimMeters.y : 1);

            dimMeters *= adjustmentRatio;

            Vector2 currentOffsetMeters = (dimMeters + new Vector2(marginMeters, marginMeters)) * new Vector2(Math.Sign(initialOffset.x), Math.Sign(initialOffset.y));

            // Create a new plane object, set its parent to the current transform, and apply the image texture
            GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newPlane.transform.parent = transform;
            ApplyImageTexture(newPlane, imageData, adjustmentRatio);

            // Set the plane's local position, local scale, and rotation
            newPlane.transform.localRotation = defaultRotation;
            newPlane.transform.localScale = new(dimMeters.x / 10f, 0.1f, dimMeters.y / 10f);
            newPlane.transform.localPosition = new(initialOffset.x + currentOffsetMeters.x / 2, initialOffset.y + currentOffsetMeters.y / 2, 0f);

            initialOffset += currentOffsetMeters;
            ret = ret ?? dimMeters/2;
        }
        return ret ?? Vector2.zero;
    }

    private async void ApplyImageTexture(GameObject plane, ImageData imageData, Vector2 adjustmentRatio)
    {
        try
        {
            Texture2D texture = await DownloadTextureAsync(imageData);
            Material planeMaterial = plane.GetComponent<Renderer>().material;
            planeMaterial.mainTexture = texture;
            planeMaterial.mainTextureScale = adjustmentRatio;

            // Pass the visible width and height values in pixels to the initialization method
            TextureScroll textureScroll = plane.AddComponent<TextureScroll>();
            textureScroll.Initialize(plane, adjustmentRatio);
        }
        catch (Exception e)
        {
            Debug.Log("Error downloading texture: " + e.Message);
            plane.GetComponent<Renderer>().material.mainTexture = errorTexture;
        }
    }

    private async Task<Texture2D> DownloadTextureAsync(ImageData imageData)
    {
        using (HttpClient client = new())
        {
            // Create a new texture and load the image data into it
            Texture2D texture = new(1, 1);//imageData.height/imageData.width);
            // Get the image data from the specified URL
            texture.LoadImage(await client.GetByteArrayAsync(imageData.url));

            return texture;
        }
    }
}