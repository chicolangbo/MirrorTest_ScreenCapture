using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("UIManager");
                    _instance = singletonObject.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    public RectTransform mainScreen;
    public int ratioX;
    public int ratioY;
    public Vector2 ClientProfileSize { get; private set; }

    private UIVerticalRatioSetter uiVerticalRatioSetter;

    private void OnEnable()
    {
        uiVerticalRatioSetter = GetComponent<UIVerticalRatioSetter>();
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIFrameSet, uiVerticalRatioSetter.SetUISize);
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUISet, SetMainScreenSize);
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUISet, SetClientProfileSize);
    }

    private void Start()
    {
        Debug.Log("UIManager : Start");
        EventManager.instance.RunEvent(CallBackEventType.TYPES.OnUIFrameSet);
        EventManager.instance.RunEvent(CallBackEventType.TYPES.OnUISet);
    }

    public void SetMainScreenSize()
    {
        Debug.Log("UIManager : SetMainScreenSize");
        var height = uiVerticalRatioSetter.uiObjects[1].sizeDelta.y - 50;
        mainScreen.sizeDelta = Utils.SetImageSizeByRatio(height, ratioX, ratioY);
    }

    public void SetClientProfileSize()
    {
        Debug.Log("UIManager : SetClientProfileSize");
        var height = uiVerticalRatioSetter.uiObjects[2].sizeDelta.y - 20;
        ClientProfileSize = Utils.SetImageSizeByRatio(height, ratioX, ratioY);
    }
}
