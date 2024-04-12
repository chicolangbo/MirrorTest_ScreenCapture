using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVerticalRatioSetter : MonoBehaviour
{
    public List<RectTransform> uiObjects = new List<RectTransform>();
    public List<float> ratio = new List<float>();

    private int curScreenHeight;

    private void OnEnable()
    {
        Debug.Log("UIVerticalRatioSetter");
        curScreenHeight = Display.main.systemHeight;
        //EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIStarted, SetUISize);
    }

    private void OnDisable()
    {
        //EventManager.instance.RemoveCallBackEvent(CallBackEventType.TYPES.OnUIStarted, SetUISize);
    }

    private void Start()
    {
        //curScreenHeight = Display.main.systemHeight;
        //if (EventManager.instance.GetCallBackEventCount<EventManager.CallBackEvent>(CallBackEventType.TYPES.OnUIStarted) >= 2)
        //{
        //    EventManager.instance.RunEvent(CallBackEventType.TYPES.OnUIStarted);
        //}
    }

    public void SetUISize()
    {
        Debug.Log("SetUISize");
        for (int i = 0; i < uiObjects.Count; ++i)
        {
            var sizeDelta = uiObjects[i].sizeDelta;
            sizeDelta.y = curScreenHeight * ratio[i];
            uiObjects[i].sizeDelta = sizeDelta;
        }
    }
}
