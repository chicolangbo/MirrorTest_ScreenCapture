using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Client
{
    public int num;
    public NetworkIdentity sender;
    public string clientName;

    public int stageNum;
    public bool isDone;
}

public class StageManager : NetworkBehaviour
{
    public List<string> tempNames = new List<string>();
    public List<Texture2D> sprites;

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

    public class SyncListClient : SyncDictionary<NetworkIdentity, Client> { }
    public readonly SyncListClient clients = new SyncListClient();

    public void CmdRegisterClients(NetworkIdentity ni)
    {
        Debug.Log("StageManager : CmdRegisterUserTask");
        if (!clients.ContainsKey(ni))
        {
            clients.Add(ni, new Client { num = clients.Count, clientName = ni.GetComponent<UserState>().clientNameCash, sender = null });
            Debug.Log($"CmdRegisterUserTask : {clients.Count}");

            // for image sending
            //var texture = sprites[clients.Count - 1];
            //Color[] pixels = texture.GetPixels();
            //Texture2D nonReadableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            //nonReadableTexture.SetPixels(pixels);
            //nonReadableTexture.Apply();
            //var bytes = nonReadableTexture.EncodeToPNG();
            //CmdSetClientDefaultImage(ni, bytes);
        }
    }

    public void CmdUnregisterClients(NetworkIdentity ni)
    {
        if (clients.ContainsKey(ni))
        {
            clients.Remove(ni);
        }
    }

    public void CmdCheckAllUsersDone()
    {
        if (isServer)
        {

            // check all users done
            foreach (var cl in clients)
            {
                if (cl.Key.GetComponent<UserState>().isDone)
                {
                    continue;
                }
                else
                {
                    return;
                }
            }

            // set next stage
            foreach (var cl in clients)
            {
                RpcNotifyAllUsersDone(cl.Key);
            }
        }
    }

    public void CmdSetClientName(NetworkIdentity id)
    {
        foreach(var cl in clients)
        {
            if(cl.Key == id)
            {
                RpcSetUserName(id, cl.Value.clientName);
                break;
            }
        }
    }

    public void CmdSetClientDefaultImage(NetworkIdentity id, byte[] texture)
    {
        TargetRpcSetUserDefaultImage(id.connectionToClient, id, texture);
    }

    public void CmdChangeSender(NetworkIdentity targetPlayer, NetworkIdentity reciever)
    {
        if (!clients.ContainsKey(reciever))
        {
            Debug.Log($"등록되지 않은 클라이언트 {reciever}");
            return;
        }

        Debug.Log($"타겟 바뀜 : {clients[reciever].sender} -> {targetPlayer}");

        var prevTarget = clients[reciever].sender;
        var isTargetChanged = (prevTarget != targetPlayer && prevTarget != null);
        if (isTargetChanged)
        {
            TargetRpcSendStopVideo(prevTarget.connectionToClient, prevTarget, reciever);

        }
        if(targetPlayer != reciever)
        {
            clients[reciever].sender = targetPlayer;
        }
        else
        {
            clients[reciever].sender = null;
        }
    }

    [ClientRpc]
    private void RpcNotifyAllUsersDone(NetworkIdentity ni)
    {
        UIManager.Instance.WaitPanelActive(false);

        var ut = ni.GetComponent<UserState>();
        if (ut == null)
        {
            Debug.Log("user task null");
            return;
        }

        ut.SetNextStage();
    }

    [ClientRpc]
    private void RpcSetUserName(NetworkIdentity ni, string name)
    {
        Debug.Log("RpcSetUserName");

        var ut = ni.GetComponent<UserState>();
        if (ut == null)
        {
            Debug.Log("user task null");
            return;
        }

        if (ut.clientName.text == "Name")
        {
            ut.clientName.text = ut.clientNameCash;
        }
    }

    [TargetRpc]
    private void TargetRpcSetUserDefaultImage(NetworkConnection connection, NetworkIdentity ni, byte[] texture)
    {
        Debug.Log("RpcSetUserDefaultImage");

        var ui = ni.GetComponent<UserImage>();
        if(ui == null)
        {
            Debug.Log("User Image null");
            return;
        }

        if(ui.defaultTexture == null)
        {
            ui.SetDefaultSprite(texture);
        }
    }

    [TargetRpc]
    private void TargetRpcSendStopVideo(NetworkConnection sender, NetworkIdentity prevTarget, NetworkIdentity reciever)
    {
        Debug.Log($"sender : {prevTarget} / 받는이 : {reciever}");
        if(prevTarget == reciever)
        {
            return;
        }
        var uv = prevTarget.GetComponent<UserVideo>();
        uv.SendStop(reciever);
    }
}
