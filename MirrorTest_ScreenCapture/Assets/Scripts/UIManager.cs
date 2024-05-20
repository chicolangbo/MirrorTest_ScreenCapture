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

    public RectTransform mainScreenBg;
    private RectTransform mainScreen;
    public int ratioX;
    public int ratioY;
    public Vector2 ClientProfileSize { get; private set; }

    private UIVerticalRatioSetter uiVerticalRatioSetter;

    private void OnEnable()
    {
        uiVerticalRatioSetter = GetComponent<UIVerticalRatioSetter>();
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIFrameSet, uiVerticalRatioSetter.SetUISize);
        
        //for imageScene
        //EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUISet, SetMainScreenSize);
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUISet, SetClientProfileSize);
    }

    private void Start()
    {
        Debug.Log("UIManager : Start");
        EventManager.instance.RunEvent(CallBackEventType.TYPES.OnUIFrameSet);
        EventManager.instance.RunEvent(CallBackEventType.TYPES.OnUISet);
    }

    private void Update()
    {
    }

    private void OnDisable()
    {
        EventManager.instance.RemoveCallBackEvent(CallBackEventType.TYPES.OnUIFrameSet, uiVerticalRatioSetter.SetUISize);

        //for imageScene
        //EventManager.instance.RemoveCallBackEvent(CallBackEventType.TYPES.OnUISet, SetMainScreenSize);
        EventManager.instance.RemoveCallBackEvent(CallBackEventType.TYPES.OnUISet, SetClientProfileSize);
    }

    public void SetMainScreenSize()
    {
        Debug.Log("UIManager : SetMainScreenSize");
        var height = uiVerticalRatioSetter.uiObjects[1].sizeDelta.y - 50;
        var bgSize = Utils.GetImageSizeByRatio(height, ratioX, ratioY);
        mainScreenBg.sizeDelta = bgSize;
        mainScreen = mainScreenBg.GetChild(0).GetComponent<RectTransform>();
        mainScreen.sizeDelta = Utils.GetChildRectAdjusted(bgSize, 10f);
    }

    public void SetClientProfileSize()
    {
        Debug.Log("UIManager : SetClientProfileSize");
        var height = uiVerticalRatioSetter.uiObjects[2].sizeDelta.y - 20;
        Debug.Log($"height : {height}");
        ClientProfileSize = Utils.GetImageSizeByRatio(height, ratioX, ratioY);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
