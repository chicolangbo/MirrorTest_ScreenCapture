using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Linq;

public class UserState : NetworkBehaviour
{
    public bool isDone;
    public int stageNum = 1;

    private Transform contents;

    private static string working = "Working...";
    private static string done = "Done";
    private static string myColor = "#FFEE4C";

    [SyncVar]
    public string clientNameCash;

    public TextMeshProUGUI clientName;
    public TextMeshProUGUI stage;
    public TextMeshProUGUI workState;
    public NetworkIdentity id;

    private void Awake()
    {
        Debug.Log("UserTask : Awake");
        contents = GameObject.FindGameObjectWithTag("Contents").GetComponent<Transform>();
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

    public override void OnStartServer()
    {
        clientNameCash = (string)connectionToClient.authenticationData;
        Debug.Log($"UserState : OnStartServer : {clientNameCash}");
    }

    private void Start()
    {
        Debug.Log("UserTask : Start");
        SetParent();
        SetColor();

        if(isClient && isLocalPlayer)
        {
            //clientName.text = "You";
            CmdRegisterUserTaskOnServer();
            CmdSetClientName(id);
        }
        else if(isServer)
        {
            StageManager.Instance.CmdRegisterClients(id);
            CmdSetClientName(id);
        }
        else
        {
            CmdSetClientName(id);
        }
    }

    public void SetParent()
    {
        transform.SetParent(contents);
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = UIManager.Instance.UIRatioSetter.ClientProfileSize;
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
            var bg = GetComponent<Image>();
            Color newColor;
            if (ColorUtility.TryParseHtmlString(myColor, out newColor))
            {
                bg.color = newColor;
            }
        }
    }

    [Command(requiresAuthority = true)]
    public void CmdUpdateWorkState()
    {
        Debug.Log("UserTask : CmdUpdateWorkState");

        isDone = true; // in server
        stageNum++;

        RpcReceiveWorkState();

        // 서버에 직접 모든 클라이언트 상태를 확인하도록 요청
        StageManager.Instance.CmdCheckAllUsersDone(); // after work state update, state check
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
        stage.text = $"stage {stageNum}";
        workState.text = working;
    }

    [Command(requiresAuthority = true)]
    private void CmdRegisterUserTaskOnServer()
    {
        Debug.Log("CmdRegisterUserTaskOnServer");
        if(isLocalPlayer)
        {
            Debug.Log("CmdRegisterUserTaskOnServer local");
            StageManager.Instance.CmdRegisterClients(id);
        }
        else
        {
            Debug.Log("CmdRegisterUserTaskOnServer not local");
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSetClientName(NetworkIdentity id)
    {
        Debug.Log("CmdSetClientName");
        StageManager.Instance.CmdSetClientName(id);
    }

    [Command(requiresAuthority = true)]
    private void CmdUnregisterUserTaskOnServer()
    {
        if(isLocalPlayer)
        {
            StageManager.Instance.CmdUnregisterClients(id);
        }
        Debug.Log("CmdUnregisterUserTaskOnServer");
    }
}
