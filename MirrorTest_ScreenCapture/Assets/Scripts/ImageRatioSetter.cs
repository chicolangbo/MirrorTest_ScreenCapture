using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRatioSetter : MonoBehaviour
{
    public List<RectTransform> images;

    public int ratioX;
    public int ratioY;

    public void SetImageSize()
    {
        Debug.Log("ImageRatioSetter : SetImageSize");

        for (int i = 0; i < images.Count; i++)
        {
            var sizeDelta = images[i].sizeDelta;
            sizeDelta.x = sizeDelta.y / ratioY * ratioX;
            images[i].sizeDelta = sizeDelta;
        }
    }
}