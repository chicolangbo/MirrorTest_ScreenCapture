using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
    [SyncVar]
    public List<UserTask> userTasks = new List<UserTask>();


    [Command]
    public void CmdRegisterUserTask(UserTask userTask)
    {
        if (!userTasks.Contains(userTask))
            {
                userTasks.Add(userTask);
            }

    }

    [Command]
    public void CmdUnregisterUserTask(UserTask userTask)
    {
        if (userTasks.Contains(userTask))
        {
            userTasks.Remove(userTask);
        }
    }

    //[Command]
    public void CmdCheckAllUsersDone()
    {
        if(isServer)
        {
            if (userTasks != null && userTasks.All(ut => ut.isDone))
            {
                Debug.Log($"All users are done! {userTasks.Count}");
                RpcNotifyAllUsersDone();
            }
        }
    }

    [ClientRpc]
    private void RpcNotifyAllUsersDone()
    {
        UIManager.Instance.WaitPanelActive(false);
        foreach(var userTask in userTasks)
        {
            if (userTask.isDone)
            {
                userTask.SetNextStage();
            }
        }
    }
}
