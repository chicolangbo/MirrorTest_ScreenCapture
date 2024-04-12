using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    public ImageRatioSetter imageRatioSetter;
    public UIVerticalRatioSetter uiVerticalRatioSetter;

    private void OnEnable()
    {
    }

    private void Start()
    {
        Debug.Log("UIManager");
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIStarted, uiVerticalRatioSetter.SetUISize);
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIStarted, SetScreenHeight);
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIStarted, imageRatioSetter.SetImageSize);

        EventManager.instance.RunEvent(CallBackEventType.TYPES.OnUIStarted);
    }

    public void SetScreenHeight()
    {
        var height = uiVerticalRatioSetter.uiObjects[2].sizeDelta.y;
        height -= 37;
        for(int i = 1; i< imageRatioSetter.images.Count; ++i)
        {
            var tempRect = imageRatioSetter.images[i].sizeDelta;
            tempRect.y = height;
            imageRatioSetter.images[i].sizeDelta = tempRect;
        }
    }
}
