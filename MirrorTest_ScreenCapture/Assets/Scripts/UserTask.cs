using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

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
    }

    private void OnDisable()
    {
        Debug.Log("UserTask : OnDisable");
        UIManager.Instance.CompletionBtn.onClick.RemoveListener(CmdUpdateWorkState);

        if (isClient && authority)
        {
            CmdUnregisterUserTaskOnServer();
        }
    }

    private void Start()
    {
        Debug.Log("UserTask : Start");
        SetParent();
        SetColor();

        if (isClient && authority)
        {
            CmdRegisterUserTaskOnServer();
        }
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

        // 자식도 맞춰줘야 함
        var childRect = rect.GetChild(0).GetComponent<RectTransform>();
        childRect.sizeDelta = Utils.GetChildRectAdjusted(rect.sizeDelta, 10f);
    }

    public void SetColor()
    {
        if(isLocalPlayer)
        {
            Debug.Log("is local player");
            var bg = GetComponent<Image>();
            Color newColor;
            if (ColorUtility.TryParseHtmlString(myColor, out newColor))
            {
                bg.color = newColor;
            }
        }
    }

    [Command]
    public void CmdUpdateWorkState()
    {
        Debug.Log("UserTask : CmdUpdateWorkState");

        isDone = true; // in server
        stageNum++;

        RpcReceiveWorkState();

        // 서버에 직접 모든 클라이언트 상태를 확인하도록 요청
        stageManager.CmdCheckAllUsersDone(); // after work state update, state check
    }

    [ClientRpc]
    public void RpcReceiveWorkState()
    {
        Debug.Log("UserTask : RpcReceiveWorkState");

        if(!isDone) // host 이중 방지
        {
            isDone = true;
            stageNum++;
        }
        workState.text = done;

        if (isLocalPlayer)
        {

            UIManager.Instance.SetNextStage(stageNum);
        }
    }

    public void SetNextStage()
    {
        Debug.Log("SetNextStage");
        isDone = false;
        workStage.text = $"stage {stageNum}";
        workState.text = working;
    }

    [Command]
    private void CmdRegisterUserTaskOnServer()
    {
        stageManager.CmdRegisterUserTask(GetComponent<NetworkIdentity>());
    }

    [Command]
    private void CmdUnregisterUserTaskOnServer()
    {
        stageManager.CmdUnregisterUserTask(GetComponent<NetworkIdentity>());
    }

}
