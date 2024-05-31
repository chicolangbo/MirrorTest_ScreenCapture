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

    private void Update()
    {
        //screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //screenTexture.Apply();
    }

    public void SendOn(NetworkIdentity targetPlayer)
    {
        if (isLocalPlayer/*targetPlayer == connectionToClient.identity*/)
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
            CmdSendCapture(targetPlayer, connectionToClient);
        }
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
    public void CmdSendCapture(NetworkIdentity targetPlayer, NetworkConnectionToClient reciever = null)
    {
        Debug.Log($"CmdSendCapture 캡처대상 {targetPlayer} / 호출한 애 {reciever}");
        var userVideo = targetPlayer.GetComponent<UserVideo>();
        //userVideo.RegisterChanel(reciever.identity);
        userVideo.TargetStartCapture(connectionToClient, reciever.identity);
    }

    public void RegisterChanel(NetworkIdentity ni)
    {
        if (!chanel.ContainsKey(ni))
        {
            chanel.Add(ni, true);
        }
    }

    [TargetRpc]
    public void TargetStartCapture(NetworkConnection senderConnection, NetworkIdentity reciever)
    {
        if (isLocalPlayer)
        {
            Debug.Log("TargetStartCapture : is LocalPlayer");
            StartCoroutine(SendCapture(reciever));
        }
    }

    public IEnumerator SendCapture(NetworkIdentity targetid)
    {
        Debug.Log("SendCapture");

        RegisterChanel(targetid);
        chanel[targetid] = true;

        while (true/*streamingState == StreamingState.Sending*/)
        {
            if (!chanel[targetid])
            {
                Debug.Log("전송 중단");
                CmdTargetUpdateScreen(targetid, null);
                break;
            }

            yield return waitForEndOfFrame;

            // 화면 캡처하여 Texture2D로 저장
            screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenTexture.Apply();

            CmdTargetUpdateScreen(targetid, screenTexture.EncodeToJPG());

            Debug.Log("Screen captured and texture updated.");
        }
    }

    [Command]
    private void CmdTargetUpdateScreen(NetworkIdentity reciever, byte[] screenData)
    {
        TargetUpdateScreen(reciever.connectionToClient, screenData);
    }

    //private void UpdateScreenDefault()
    //{
    //    Debug.Log("UpdateScreenDefault");
    //    screenTexture = defaultTexture;
    //    mainScreenSetter.SetMainScreen(screenTexture);
    //}


    [TargetRpc] // 특정 클라이언트에게 화면 데이터 전송
    private void TargetUpdateScreen(NetworkConnection targetConnection, byte[] screenData)
    {
        Debug.Log("TargetUpdateScreen");

        if(screenData != null)
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
