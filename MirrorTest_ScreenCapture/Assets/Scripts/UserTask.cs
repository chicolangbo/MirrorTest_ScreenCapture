using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UserTask : NetworkBehaviour
{
    [SyncVar]
    public bool isDone;

    private List<Toggle> tasks = new List<Toggle>();
    private Button CompletionBtn;
    private Transform contents;

    private static string working = "Working...";
    private static string done = "Done";
    private static string myColor = "#FFEE4C";
    private TextMeshProUGUI workState;

    private void Awake()
    {
        Debug.Log("UserTask : Awake");

        var task = GameObject.FindGameObjectWithTag("Task");
        var taskObjs = task.GetComponentsInChildren<Toggle>();
        foreach(var t in taskObjs)
        {
            t.onValueChanged.AddListener( delegate { CheckCompletion(t); });
            tasks.Add(t);
        }

        CompletionBtn = GameObject.FindGameObjectWithTag("SendBtn").GetComponent<Button>();

        contents = GameObject.FindGameObjectWithTag("Contents").GetComponent<Transform>();

        workState = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        Debug.Log("UserTask : OnEnable");
        CompletionBtn.onClick.AddListener(CmdUpdateWorkState);
    }

    private void OnDisable()
    {
        Debug.Log("UserTask : OnDisable");
        CompletionBtn.onClick.RemoveListener(CmdUpdateWorkState);
    }

    private void Start()
    {
        Debug.Log("UserTask : Start");
        SetParent();
        SetColor();
    }

    public void SetParent()
    {
        transform.SetParent(contents);
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = UIManager.Instance.ClientProfileSize;
        rect.localScale = new Vector3(1, 1, 1);
        var tempPos = rect.position;
        tempPos.z = 0f;
        rect.localPosition = tempPos;

        // ÀÚ½Äµµ ¸ÂÃçÁà¾ß ÇÔ
        var childRect = rect.GetChild(0).GetComponent<RectTransform>();
        childRect.sizeDelta = Utils.GetChildRectAdjusted(rect.sizeDelta, 10f);
    }

    public void SetColor()
    {
        if(isLocalPlayer)
        {
            Debug.Log("is local player");
            var bg = GetComponent<Image>();
            Debug.Log(bg==null);
            Debug.Log(bg.color);
            Color newColor;
            if (ColorUtility.TryParseHtmlString(myColor, out newColor))
            {
                Debug.Log(bg.color);
                bg.color = newColor;
            }
        }
    }


    public bool IsTaskDone(Toggle changedValue)
    {
        return tasks.All(toggle => toggle.isOn);
    }

    public void CheckCompletion(Toggle changedValue)
    {
        if(IsTaskDone(changedValue))
        {
            Debug.Log("CheckCompletion => complete!");
            isDone = true;
            CompletionBtn.interactable = true;
        }
        Debug.Log("CheckCompletion => not yet!");
    }

    [Command]
    public void CmdUpdateWorkState()
    {
        Debug.Log("UserTask : CmdUpdateWorkState");
        RpcReceiveWorkState();
    }

    [ClientRpc]
    public void RpcReceiveWorkState()
    {
        Debug.Log("UserTask : RpcReceiveWorkState");
        workState.text = done;
    }
}
