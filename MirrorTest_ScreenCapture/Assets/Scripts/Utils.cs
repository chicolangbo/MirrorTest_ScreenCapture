using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2 SetImageSizeByRatio(float height, float ratioX, float ratioY)
    {
        Debug.Log("Utils : SetImageSizeByRatio");
        float offset = 100f;
        var sizeDelta = new Vector2();
        sizeDelta.x = height / ratioY * ratioX;
        sizeDelta.y = height;

        if(sizeDelta.x >= Display.main.systemWidth - offset)
        {
            sizeDelta.x = Display.main.systemWidth - offset;
            // y값 맞춰주기
            sizeDelta.y = sizeDelta.x * ratioY / ratioX;
        }

        return sizeDelta;
    }
}
