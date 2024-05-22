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
public class UIPanelWithVerticalRatio
{
    public RectTransform rectTr;
    public float verticalRatio;
}

[Serializable]
public class UIPanelWithHorizontalRatio
{
    public RectTransform rectTr;
    public UIPanel panelType;
    public float horizontalRatio;
}


public class UIRatioSetter : MonoBehaviour
{
    // uiPanel, vertical ratio
    public List<UIPanelWithVerticalRatio> uiPanelWithVerticalRatio = new List<UIPanelWithVerticalRatio>();

    // uiObject, horizontal ratio
    public List<UIPanelWithHorizontalRatio> uiPanelWithHorizontalRatio = new List<UIPanelWithHorizontalRatio>();

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
    
    public void SetUISize_Depth1()
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

    public void SetUISize_Depth2()
    {
        for (var i = 0; i < uiPanelWithHorizontalRatio.Count; ++i)
        {
            // size setting
            var panelType = (int)uiPanelWithHorizontalRatio[i].panelType;
            var height = uiPanelWithVerticalRatio[panelType].rectTr.rect.height;
            var width = height * uiPanelWithHorizontalRatio[i].horizontalRatio;
            var tempSize = new Vector2(width,height);
            uiPanelWithHorizontalRatio[i].rectTr.sizeDelta = tempSize;
        }
    }

    public void SetClientProfileSize()
    {
        var height = uiPanelWithVerticalRatio[2].rectTr.sizeDelta.y - 20;
        ClientProfileSize = Utils.GetImageSizeByRatio(height, clientRatio.x, clientRatio.y);
    }
}
