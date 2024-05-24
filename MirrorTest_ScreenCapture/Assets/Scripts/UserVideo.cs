using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StreamingState
{
    Start,
    Sending,
    Stop
}

public class UserVideo : NetworkBehaviour
{
    public StreamingState streamingState;

    private Texture2D screenTexture;
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private MainScreenSetter mainScreenSetter;
    private NetworkIdentity reciever;

    private void OnEnable()
    {
        if(screenTexture == null)
        {
            screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        }
    }

    private void OnDisable()
    {
        if(screenTexture != null)
        {
            Destroy(screenTexture);
        }
    }

    private void Start()
    {
        if(mainScreenSetter == null)
        {
            mainScreenSetter = GameObject.FindGameObjectWithTag("MainScreenSetter").GetComponent<MainScreenSetter>();
        }
    }

    private void Update()
    {
        //if(streamingState == StreamingState.Start)
        //{
        //    StartCoroutine(SendCapture());
        //}
    }

    public void SendCaptureTrigger(NetworkIdentity targetPlayer)
    {
        if (isLocalPlayer)
        {
            return;
        }
        else
        {
            CmdSendCapture(targetPlayer, connectionToClient);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSendCapture(NetworkIdentity targetPlayer, NetworkConnectionToClient reciever = null)
    {
        Debug.Log("CmdSendCapture");
        targetPlayer.GetComponent<UserVideo>().StartCoroutine(SendCapture(reciever));
    }

    [Server]
    public IEnumerator SendCapture(NetworkConnection targetConnection)
    {
        Debug.Log("SendCapture");

        streamingState = StreamingState.Sending;

        while (streamingState == StreamingState.Sending)
        {
            yield return waitForEndOfFrame;

            // 화면 캡처하여 Texture2D로 저장
            screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenTexture.Apply();

            TargetUpdateScreen(targetConnection, screenTexture.EncodeToJPG());

            Debug.Log("Screen captured and texture updated.");
        }
    }

    [TargetRpc] // 특정 클라이언트에게 화면 데이터 전송
    private void TargetUpdateScreen(NetworkConnection targetConnection, byte[] screenData)
    {
        Debug.Log("RpcUpdateScreen");

        screenTexture.LoadImage(screenData);
        mainScreenSetter.SetMainScreen(screenTexture);
    }

    public void SendOn()
    {
        //streamingState = StreamingState.Start;
        //NetworkIdentity localPlayer = NetworkClient.connection.identity;
        //if(localPlayer != null)
        //{
        //    var userVideo = localPlayer.GetComponent<UserVideo>();
        //    if (userVideo != null)
        //    {
        //        userVideo.CmdSendCapture()
        //    }
        //}
    }

    public void SendStop()
    {
        streamingState = StreamingState.Stop;
    }
}
