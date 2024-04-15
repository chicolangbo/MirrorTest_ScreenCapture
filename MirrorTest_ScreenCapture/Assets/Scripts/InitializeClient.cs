using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeClient : MonoBehaviour
{
    private Transform parent;
    private RectTransform rect;

    public int ratioX;
    public int ratioY;

    private void Awake()
    {
        Debug.Log("InitializeClient : Awake");

        parent = GameObject.FindGameObjectWithTag("ClientParent").transform;
        rect.SetParent(parent);
        //Utils.SetImageHeight(rect, UIManager.instance.clientHeight);
        Utils.SetImageSizeByRatio(rect, ratioX, ratioY);
    }

}
