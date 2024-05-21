using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;

public class UserTask : NetworkBehaviour
{
    public bool isDone;
    public int stageNum = 1;

    private Transform contents;

    private static string working = "Working...";
    private static string done = "Done";
    private static string myColor = "#FFEE4C";
    private TextMeshProUGUI workStage;
    private TextMeshProUGUI workState;

    //[SyncVar]
    //public List<UserTask> userTasks = new List<UserTask>();
    private StageManager stageManager;

    private void Awake()
    {
        Debug.Log("UserTask : Awake");
        contents = GameObject.FindGameObjectWithTag("Contents").GetComponent<Transform>();
        workStage = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        workState = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        stageManager = GameObject.FindGameObjectWithTag("StageManager").GetComponent<StageManager>();
    }

    private void OnEnable()
    {
        Debug.Log("UserTask : OnEnable");
        UIManager.Instance.CompletionBtn.onClick.AddListener(CmdUpdateWorkState);
        stageManager.CmdRegisterUserTask(this);
    }

    private void OnDisable()
    {
        Debug.Log("UserTask : OnDisable");
        UIManager.Instance.CompletionBtn.onClick.RemoveListener(CmdUpdateWorkState);
        stageManager.CmdUnregisterUserTask(this);
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

        if(isLocalPlayer)
        {
            isDone = true;
            stageNum++;
            UIManager.Instance.SetNextStage(stageNum);
            //stageManager.CmdCheckAllUsersDone();
        }
    }

    public void SetNextStage()
    {
        isDone = false;
        workStage.text = $"stage {stageNum}";
        workState.text = working;
    }

    ////[Command]
    //public void CmdRegisterUserTask(UserTask userTask)
    //{
    //    if (!userTasks.Contains(userTask))
    //    {
    //        userTasks.Add(userTask);
    //    }
    //}

    ////[Command]
    //public void CmdUnregisterUserTask(UserTask userTask)
    //{
    //    if (userTasks.Contains(userTask))
    //    {
    //        userTasks.Remove(userTask);
    //    }
    //}

    ////[Command]
    //public void CmdCheckAllUsersDone()
    //{
    //    if (userTasks != null && userTasks.All(ut => ut.isDone))
    //    {
    //        Debug.Log($"All users are done! {userTasks.Count}");
    //        RpcNotifyAllUsersDone();
    //    }
    //}

    //[ClientRpc]
    //private void RpcNotifyAllUsersDone()
    //{
    //    UIManager.Instance.WaitPanelActive(false);
    //}
}
