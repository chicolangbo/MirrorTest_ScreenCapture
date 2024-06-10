using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UserVideo : NetworkBehaviour
{
    public Dictionary<NetworkIdentity, bool> chanel = new Dictionary<NetworkIdentity, bool>();

    public Texture2D screenTexture;
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private MainScreenSetter mainScreenSetter;  
    public NetworkIdentity id;

    private NetworkIdentity prevTarget;

    //public Sprite defaultSprite;

    private void OnEnable()
    {
        if (screenTexture == null)
        {
            screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        }
    }

    private void OnDisable()
    {
        if (screenTexture != null)
        {
            Destroy(screenTexture);
        }
    }

    private void Start()
    {
        if (mainScreenSetter == null)
        {
            mainScreenSetter = GameObject.FindGameObjectWithTag("MainScreenSetter").GetComponent<MainScreenSetter>();
        }
        id = GetComponent<NetworkIdentity>();
    }

    public void SendOn(NetworkIdentity targetPlayer)
    {
        if (isLocalPlayer)
        {
            // 자기 자신을 클릭한 경우
            // texture null로 바꿔버리기
            Debug.Log("자기자신 클릭");
            CmdChangeSender(targetPlayer, connectionToClient);
        }
        else
        {
            //CmdSendStopOthers();
            CmdChangeSender(targetPlayer, connectionToClient);

            if(!prevTarget == targetPlayer)
            {
                Debug.Log("SendOn");
                CmdRequestScreen(targetPlayer, connectionToClient);
            }
        }
        prevTarget = targetPlayer;
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeSender(NetworkIdentity targetPlayer, NetworkConnectionToClient reciever = null)
    {
        StageManager.Instance.CmdChangeSender(targetPlayer, reciever.identity);
    }

    public void SendStop(NetworkIdentity reciever)
    {
        Debug.Log("SendStop");
        if (!chanel.ContainsKey(reciever))
        {
            Debug.Log("등록되지 않은 채널에서 삭제하려고 함");
            return;
        }
        chanel[reciever] = false;
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestScreen(NetworkIdentity target, NetworkConnectionToClient recieverConnection = null)
    {
        Debug.Log("CmdRequestScreen : " + recieverConnection.identity);
        var userVideo = target.GetComponent<UserVideo>();
        userVideo.TargetStartCapture(target.connectionToClient, recieverConnection.identity);
    }

    public void RegisterChanel(NetworkIdentity recieverConnection)
    {
        Debug.Log("RegisterChanel, networkconnection : " + recieverConnection);
        if (!chanel.ContainsKey(recieverConnection))
        {
            chanel.Add(recieverConnection, true);
        }
    }

    [TargetRpc]
    public void TargetStartCapture(NetworkConnection senderConnection, NetworkIdentity reciever)
    {
        if (isLocalPlayer)
        {
            Debug.Log("TargetStartCapture, sender : " + senderConnection);
            Debug.Log("TargetStartCapture, reciever : " + reciever);
            StartCoroutine(CaptureAndSend(reciever));
        }
    }

    public IEnumerator CaptureAndSend(NetworkIdentity targetID)
    {
        RegisterChanel(targetID);
        chanel[targetID] = true;
        var target = targetID.GetComponent<UserVideo>();

        while (true)
        {
            if (!chanel[targetID])
            {
                Debug.Log("전송 중단");
                target.CmdTargetUpdateScreen(targetID, null);
                break;
            }

            yield return waitForEndOfFrame;

            // 화면 캡처하여 Texture2D로 저장
            screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenTexture.Apply();
            target.CmdTargetUpdateScreen(targetID, screenTexture.EncodeToJPG(50));
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdTargetUpdateScreen(NetworkIdentity targetID, byte[] screenData)
    {
        TargetUpdateScreen(targetID, screenData);
    }

    [TargetRpc] // 특정 클라이언트에게 화면 데이터 전송
    private void TargetUpdateScreen(NetworkIdentity targetID, byte[] screenData)
    {
        Debug.Log("TargetUpdateScreen");

        var targetUserVideo = targetID.GetComponent<UserVideo>();
        targetUserVideo.UpdateScreenData(screenData);
    }

    public void UpdateScreenData(byte[] screenData)
    {
        if (screenData != null)
        {
            screenTexture.LoadImage(screenData);
            mainScreenSetter.SetMainScreen(screenTexture);
        }
        else
        {
            mainScreenSetter.SetMainScreen(null);
        }
    }
}
