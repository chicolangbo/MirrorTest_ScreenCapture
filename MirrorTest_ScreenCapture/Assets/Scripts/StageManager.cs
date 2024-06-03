using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
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

    public class SyncListNetworkIdentity : SyncList<NetworkIdentity> { }
    public class SyncDictionaryIdentity : SyncDictionary<NetworkIdentity, NetworkIdentity> { }

    public readonly SyncListNetworkIdentity clientsId = new SyncListNetworkIdentity();
    public readonly SyncDictionaryIdentity clientWithSender = new SyncDictionaryIdentity();


    public void CmdRegisterClients(NetworkIdentity ni)
    {
        Debug.Log("StageManager : CmdRegisterUserTask");
        if (!clientsId.Contains(ni))
        {
            clientsId.Add(ni);
            clientWithSender.Add(ni, null);
            Debug.Log($"CmdRegisterUserTask : {clientsId.Count}");

            //2024.6.3 임시 추가

            var texture = sprites[clientsId.Count - 1];

            // 원본 텍스처의 픽셀 데이터를 복사
            Color[] pixels = texture.GetPixels();

            // 새로운 압축되지 않은 텍스처를 생성
            Texture2D nonReadableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            nonReadableTexture.SetPixels(pixels);
            nonReadableTexture.Apply();

            var bytes = nonReadableTexture.EncodeToPNG();
            CmdSetClientDefaultImage(ni, bytes);
        }

    }

    public void CmdUnregisterClients(NetworkIdentity ni)
    {
        if (clientsId.Contains(ni))
        {
            clientsId.Remove(ni);
            clientWithSender.Remove(ni);
        }
    }

    public void CmdCheckAllUsersDone()
    {
        if (isServer)
        {
            if (clientsId != null && clientsId.All(ut => ut.GetComponent<UserTask>().isDone))
            {
                Debug.Log($"All users are done! {clientsId.Count}");
                foreach (var ni in clientsId)
                {
                    if (ni == null)
                    {
                        Debug.Log("user task identity null");
                        continue;
                    }

                    RpcNotifyAllUsersDone(ni);
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
        for (int i = 0; i < clientsId.Count; ++i)
        {
            if (id == clientsId[i])
            {
                RpcSetUserName(id, i);
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
        if (!clientWithSender.ContainsKey(reciever))
        {
            Debug.Log($"등록되지 않은 클라이언트 {reciever}");
            return;
        }

        Debug.Log($"타겟 바뀜 : {clientWithSender[reciever]} -> {targetPlayer}");

        var prevTarget = clientWithSender[reciever];
        if (prevTarget != targetPlayer && prevTarget != null)
        {
            TargetRpcSendStopVideo(prevTarget.connectionToClient, prevTarget, reciever);

        }
        if(targetPlayer != reciever)
        {
            clientWithSender[reciever] = targetPlayer;
        }
        else
        {
            clientWithSender[reciever] = null;
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

        if(ui.defaultTexture != null)
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
