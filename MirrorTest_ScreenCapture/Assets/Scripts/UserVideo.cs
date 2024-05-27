using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum StreamingState
{
    Sending,
    Stop
}

public class UserVideo : NetworkBehaviour
{
    public StreamingState streamingState = StreamingState.Stop;

    private Texture2D screenTexture;
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private MainScreenSetter mainScreenSetter;
    public NetworkIdentity id;

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
        id = GetComponent<NetworkIdentity>();
    }

    private void Update()
    {
        //if(streamingState == StreamingState.Start)
        //{
        //    StartCoroutine(SendCapture());
        //}
    }

    public void SendOn(NetworkIdentity targetPlayer)
    {
        if (isLocalPlayer)
        {
            return;
        }
        else
        {
            streamingState = StreamingState.Sending;
            //CmdSendStopOthers();
            CmdSendCapture(targetPlayer, connectionToClient);
        }
    }

    public void SendStop()
    {
        streamingState = StreamingState.Stop;
    }

    [Command(requiresAuthority = false)]
    public void CmdSendStopOthers()
    {
        StageManager.Instance.CmdSendStopOthers(id);
    }

    [Command(requiresAuthority = false)]
    public void CmdSendCapture(NetworkIdentity targetPlayer, NetworkConnectionToClient reciever = null)
    {
        Debug.Log($"CmdSendCapture ĸó��� {targetPlayer} / ȣ���� �� {reciever}");
        //targetPlayer.GetComponent<UserVideo>().StartCoroutine(SendCapture(reciever));
        targetPlayer.GetComponent<UserVideo>().TargetStartCapture(connectionToClient, reciever.identity);
    }

    [TargetRpc]
    public void TargetStartCapture(NetworkConnection senderConnection, NetworkIdentity reciever)
    {
        if(isLocalPlayer)
        {
            StartCoroutine(SendCapture(reciever));
        }
        if(isServer)
        {
            Debug.Log("TargetStartCapture : is server");
        }
        if(isOwned)
        {
            Debug.Log("TargetStartCapture : is owned");
        }
        if(isClient)
        {
            Debug.Log("TargetStartCapture : is Client");
        }

    }

    public IEnumerator SendCapture(NetworkIdentity targetid)
    {
        Debug.Log("SendCapture");

        while (true/*streamingState == StreamingState.Sending*/)
        {
            yield return waitForEndOfFrame;

            // ȭ�� ĸó�Ͽ� Texture2D�� ����
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


    [TargetRpc] // Ư�� Ŭ���̾�Ʈ���� ȭ�� ������ ����
    private void TargetUpdateScreen(NetworkConnection targetConnection, byte[] screenData)
    {
        Debug.Log("RpcUpdateScreen");

        screenTexture.LoadImage(screenData);
        mainScreenSetter.SetMainScreen(screenTexture);
    }
}
