using Mirror;
using System.Linq;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
    public static StageManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public class SyncListNetworkIdentity : SyncList<NetworkIdentity> { }

    public readonly SyncListNetworkIdentity clientsId = new SyncListNetworkIdentity();


    public void CmdRegisterClients(NetworkIdentity userTask)
    {
        Debug.Log("StageManager : CmdRegisterUserTask");
        if (!clientsId.Contains(userTask))
        {
            clientsId.Add(userTask);
            Debug.Log($"CmdRegisterUserTask : {clientsId.Count}");
        }
    }

    public void CmdUnregisterClients(NetworkIdentity userTask)
    {
        if (clientsId.Contains(userTask))
        {
            clientsId.Remove(userTask);
        }
    }

    public void CmdCheckAllUsersDone()
    {
        if (isServer)
        {
            if (clientsId != null && clientsId.All(ut => ut.GetComponent<UserTask>().isDone))
            {
                Debug.Log($"All users are done! {clientsId.Count}");
                foreach (var userTask in clientsId)
                {
                    if (userTask == null)
                    {
                        Debug.Log("user task identity null");
                        continue;
                    }

                    RpcNotifyAllUsersDone(userTask);
                }
            }
            Debug.Log($"is Server : not all users{clientsId.Count}");
        }
        else
        {
            Debug.Log($"is Not Server : {clientsId.Count}");
        }
    }

    public void CmdSetClientName(NetworkIdentity id)
    {
        for(int i = 0; i < clientsId.Count; ++i)
        {
            if(id == clientsId[i])
            {
                RpcSetUserName(id, i);
                break;
            }
        }
    }

    public void CmdSendStopOthers(NetworkIdentity id)
    {
        if(isServer)
        {
            if(!clientsId.Contains(id))
            {
                Debug.Log("등록되지 않은 id");
                return;
            }
            foreach(var client in clientsId)
            {
                var userVideo = client.GetComponent<UserVideo>();
                if(client == id || userVideo.streamingState == StreamingState.Stop)
                {
                    Debug.Log($"전송할 비디오 : {id}");
                    userVideo.streamingState = StreamingState.Sending;
                    continue;
                }
                RpcSendStopVideo(client);
            }
        }
        else
        {
            Debug.Log("CmdSendStopOthers");
        }
    }

    [ClientRpc]
    private void RpcNotifyAllUsersDone(NetworkIdentity ni)
    {
        UIManager.Instance.WaitPanelActive(false);

        var ut = ni.GetComponent<UserTask>();
        if (ut == null)
        {
            Debug.Log("user task null");
            return;
        }

        ut.SetNextStage();
    }

    [ClientRpc]
    private void RpcSetUserName(NetworkIdentity ni, int i)
    {
        Debug.Log("RpcSetUserName");

        var ut = ni.GetComponent<UserTask>();
        if(ut == null)
        {
            Debug.Log("user task null");
            return;
        }

        if(ut.clientName.text == "Name")
        {
            ut.clientName.text = $"user {i}";
        }
    }

    [ClientRpc]
    private void RpcSendStopVideo(NetworkIdentity id)
    {
        var uv = id.GetComponent<UserVideo>();
        if(uv == null)
        {
            Debug.Log("user video null");
            return;
        }

        uv.SendStop();
    }
}
