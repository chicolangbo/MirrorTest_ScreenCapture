using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

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

    public List<Toggle> tasks = new List<Toggle>();
    public Transform tasksParent;
    public Button CompletionBtn;
    public TextMeshProUGUI stage;
    public GameObject waitPanel;

    public Vector2 ClientProfileSize { get; private set; }

    private UIVerticalRatioSetter uiVerticalRatioSetter;

    public int ratioX;
    public int ratioY;

    private void OnEnable()
    {
        uiVerticalRatioSetter = GetComponent<UIVerticalRatioSetter>();
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIFrameSet, uiVerticalRatioSetter.SetUISize);
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUISet, SetClientProfileSize);

        Init();
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
        EventManager.instance.RemoveCallBackEvent(CallBackEventType.TYPES.OnUISet, SetClientProfileSize);

        foreach(var t in tasks)
        {
            t.onValueChanged.RemoveListener(delegate { CheckCompletion(t); });
        }
    }

    public void Init()
    {
        // tasks
        var taskObjs = tasksParent.GetComponentsInChildren<Toggle>();
        foreach (var t in taskObjs)
        {
            tasks.Add(t);
            t.onValueChanged.AddListener(delegate { CheckCompletion(t); });
        }

        //// completion btn
        //CompletionBtn = GameObject.FindGameObjectWithTag("SendBtn").GetComponent<Button>();

        //// stage
        //stage = tasksParent.GetChild(0).GetComponent<TextMeshProUGUI>();

        //// wait
        //waitPanel = GameObject.FindGameObjectWithTag("Wait");
    }

    public void SetClientProfileSize()
    {
        Debug.Log("UIManager : SetClientProfileSize");
        var height = uiVerticalRatioSetter.uiObjects[2].sizeDelta.y - 20;
        ClientProfileSize = Utils.GetImageSizeByRatio(height, ratioX, ratioY);
    }

    public void WaitPanelActive(bool active)
    {
        waitPanel.SetActive(active);
    }

    public void SetNextStage(int n)
    {
        stage.text = $"stage {n}";

        foreach(var t in tasks)
        {
            t.isOn = false;
        }
        CompletionBtn.interactable = false;
        WaitPanelActive(true);
    }

    public bool IsTaskDone(Toggle changedValue)
    {
        return tasks.All(toggle => toggle.isOn);
    }

    public void CheckCompletion(Toggle changedValue)
    {
        if (IsTaskDone(changedValue))
        {
            Debug.Log("CheckCompletion => complete!");
            CompletionBtn.interactable = true;
        }
        else
        {
            CompletionBtn.interactable = false;
        }
        Debug.Log("CheckCompletion => not yet!");
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
