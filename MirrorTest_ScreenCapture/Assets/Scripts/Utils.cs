using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void SetImageSizeByRatio(RectTransform rect, float ratioX, float ratioY)
    {
        Debug.Log("Utils : SetImageSizeByRatio");
        var sizeDelta = rect.sizeDelta;
        sizeDelta.x = sizeDelta.y / ratioY * ratioX;
        rect.sizeDelta = sizeDelta;
    }

    public static void SetImageHeight(RectTransform rect, float height)
    {
        Debug.Log("Utils : SetImageHeight");
        var tempRect = rect.sizeDelta;
        tempRect.y = height;
        rect.sizeDelta = tempRect;
    }
}
