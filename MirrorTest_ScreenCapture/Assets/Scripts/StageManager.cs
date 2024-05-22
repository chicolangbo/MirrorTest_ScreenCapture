using Mirror;
using Mono.CecilX.Cil;
using System.Collections;
using System.Collections.Generic;
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

    public SyncListNetworkIdentity userTasks = new SyncListNetworkIdentity();


    //[Command]
    public void CmdRegisterUserTask(NetworkIdentity userTask)
    {
        Debug.Log("CmdRegisterUserTask");
        if (!userTasks.Contains(userTask))
            {
                userTasks.Add(userTask);
                Debug.Log($"CmdRegisterUserTask : {userTasks.Count}");
            }
    }

    //[Command]
    public void CmdUnregisterUserTask(NetworkIdentity userTask)
    {
        if (userTasks.Contains(userTask))
        {
            userTasks.Remove(userTask);
        }
    }

    //[Command]
    public void CmdCheckAllUsersDone()
    {
        if (isServer)
        {
            if (userTasks != null && userTasks.All(ut => ut.GetComponent<UserTask>().isDone))
            {
                Debug.Log($"All users are done! {userTasks.Count}");
                RpcNotifyAllUsersDone();
            }
            Debug.Log($"is Server : not all users{userTasks.Count}");
        }
        else
        {
            Debug.Log($"is Not Server : {userTasks.Count}");
        }
    }

    [ClientRpc]
    private void RpcNotifyAllUsersDone()
    {
        UIManager.Instance.WaitPanelActive(false);

        foreach(var userTask in userTasks)
        {
            if(userTask == null)
            {
                Debug.Log("user task identity null");
                continue;
            }

            var ut = userTask.GetComponent<UserTask>();
            if (ut == null)
            {
                Debug.Log("user task null");
                continue;
            }

            ut.SetNextStage();
        }
    }
}
