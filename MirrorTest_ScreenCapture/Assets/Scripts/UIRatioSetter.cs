using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIPanel
{
    System,
    Main,
    Client
}

[Serializable]
public class UIPanelWithRatio
{
    public RectTransform rectTr;
    public float verticalRatio;
}

[Serializable]
public class UIObjectWithRatio
{
    public RectTransform rectTr;
    public UIPanel panelType;
    public float horizontalRatio;
}


public class UIRatioSetter : MonoBehaviour
{
    // uiPanel, vertical ratio
    public List<UIPanelWithRatio> uiPanelWithVerticalRatio = new List<UIPanelWithRatio>();

    // uiObject, horizontal ratio
    public List<UIObjectWithRatio> uiObjectWithHorizontalRatio = new List<UIObjectWithRatio>();

    // client x,y ratio
    public Vector2 clientRatio;

    // for cashing
    public Vector2 ClientProfileSize { get; private set; }
    private int screenHeight;
    private int screenWidth;

    private void OnEnable()
    {
        Debug.Log("UIVerticalRatioSetter : OnEnable");
        screenHeight = (int)Screen.safeArea.height;
        screenWidth = (int)Screen.safeArea.width;
    }
    
    public void SetUIPanelSize()
    {
        float tempHeight = 0f;

        for(var i = 0; i < uiPanelWithVerticalRatio.Count; ++i)
        {
            // size setting
            var tempSize = new Vector2(screenWidth, screenHeight * uiPanelWithVerticalRatio[i].verticalRatio);
            uiPanelWithVerticalRatio[i].rectTr.sizeDelta = tempSize;

            // position setting
            if (i == 0)
            {
                var newPos = new Vector3(Screen.safeArea.xMin, Screen.safeArea.yMax, 0);
                uiPanelWithVerticalRatio[i].rectTr.position = newPos;
            }
            else
            {
                var newPos = new Vector3(0, uiPanelWithVerticalRatio[i - 1].rectTr.position.y - tempHeight, 0);
                uiPanelWithVerticalRatio[i].rectTr.position = newPos;
            }

            tempHeight = tempSize.y;
        }
    }

    public void SetUIObjectSize()
    {
        for (var i = 0; i < uiObjectWithHorizontalRatio.Count; ++i)
        {
            // size setting
            var panelType = (int)uiObjectWithHorizontalRatio[i].panelType;
            var height = uiPanelWithVerticalRatio[panelType].rectTr.rect.height;
            Debug.Log(height);
            var width = height * uiObjectWithHorizontalRatio[i].horizontalRatio;
            var tempSize = new Vector2(width,height);
            uiObjectWithHorizontalRatio[i].rectTr.sizeDelta = tempSize;
        }
    }

    public void SetClientProfileSize()
    {
        var height = uiPanelWithVerticalRatio[2].rectTr.sizeDelta.y - 20;
        ClientProfileSize = Utils.GetImageSizeByRatio(height, clientRatio.x, clientRatio.y);
    }
}
