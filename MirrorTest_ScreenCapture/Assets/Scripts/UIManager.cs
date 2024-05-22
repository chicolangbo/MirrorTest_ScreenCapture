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
    public UIRatioSetter UIRatioSetter { get; private set; }

    private void OnEnable()
    {
        UIRatioSetter = GetComponent<UIRatioSetter>();
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIFrameSet, UIRatioSetter.SetUISize_Depth1);
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUIFrameSet, UIRatioSetter.SetUISize_Depth2);
        EventManager.instance.AddCallBackEvent(CallBackEventType.TYPES.OnUISet, UIRatioSetter.SetClientProfileSize);

        Init();
        Debug.Log("ui panel set ¿Ï·á");
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
        EventManager.instance.RemoveCallBackEvent(CallBackEventType.TYPES.OnUIFrameSet, UIRatioSetter.SetUISize_Depth1);
        EventManager.instance.RemoveCallBackEvent(CallBackEventType.TYPES.OnUIFrameSet, UIRatioSetter.SetUISize_Depth2);
        EventManager.instance.RemoveCallBackEvent(CallBackEventType.TYPES.OnUISet, UIRatioSetter.SetClientProfileSize);

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
