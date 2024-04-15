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
        Debug.Log("UIVerticalRatioSetter : OnEnable");
        curScreenHeight = Display.main.systemHeight;
    }

    public void SetUISize()
    {
        Debug.Log("UIVerticalRatioSetter : SetUISize");
        for (int i = 0; i < uiObjects.Count; ++i)
        {
            var sizeDelta = uiObjects[i].sizeDelta;
            sizeDelta.y = curScreenHeight * ratio[i];
            uiObjects[i].sizeDelta = sizeDelta;
        }
    }
}
