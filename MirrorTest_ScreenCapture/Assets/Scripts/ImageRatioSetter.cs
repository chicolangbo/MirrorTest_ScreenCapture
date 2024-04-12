using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRatioSetter : MonoBehaviour
{
    public List<RectTransform> images;

    public int ratioX;
    public int ratioY;

    private void OnEnable()
    {
        Debug.Log("ImageRatioSetter");

        //EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIStarted, SetImageSize);
    }

    //private void OnDisable()
    //{
    //    EventManager.instance.RemoveCallBackEvent(CallBackEventType.TYPES.OnUIStarted, SetImageSize);
    //}

    //private void Start()
    //{
    //    if(EventManager.instance.GetCallBackEventCount<EventManager.CallBackEvent>(CallBackEventType.TYPES.OnUIStarted) >= 2)
    //    {
    //        EventManager.instance.RunEvent(CallBackEventType.TYPES.OnUIStarted);
    //    }
    //}

    public void SetImageSize()
    {
        Debug.Log("SetImageSize");

        for (int i = 0; i < images.Count; i++)
        {
            var sizeDelta = images[i].sizeDelta;
            Debug.Log("¹Ù²î±â Àü " + sizeDelta);
            sizeDelta.x = sizeDelta.y / ratioY * ratioX;
            images[i].sizeDelta = sizeDelta;
            Debug.Log("¹Ù²ïÈÄ " + sizeDelta);
        }
    }
}