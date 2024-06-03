using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitPanelSizeSetter : MonoBehaviour
{
    public RectTransform mainPanelTr;

    private RectTransform tr;

    private void OnEnable()
    {
        tr = GetComponent<RectTransform>();
        tr.sizeDelta = mainPanelTr.sizeDelta;
    }
}
