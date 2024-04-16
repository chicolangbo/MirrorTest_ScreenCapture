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
        float tempHeight = 0f;
        for (int i = 0; i < uiObjects.Count; ++i)
        {
            // size setting
            //var sizeDelta = uiObjects[i].sizeDelta;
            var sizeX = Display.main.systemWidth; // 전체 너비
            var tempSize = new Vector2(sizeX, curScreenHeight * ratio[i]);
            //sizeDelta.y = curScreenHeight * ratio[i];
            uiObjects[i].sizeDelta = tempSize;

            // position setting
            if(i > 0)
            {
                var newPos = new Vector3(0, uiObjects[i - 1].position.y - tempHeight, 0);
                uiObjects[i].position = newPos;
            }

            tempHeight = tempSize.y;
        }
    }
}
