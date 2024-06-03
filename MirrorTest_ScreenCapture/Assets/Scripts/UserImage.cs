using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserImage : NetworkBehaviour
{
    public NetworkIdentity id;
    public Texture2D screenTexture;
    public Texture2D defaultTexture;

    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private MainScreenSetter mainScreenSetter;

    //public Sprite defaultSprite;

    private void OnEnable()
    {
        if (screenTexture == null)
        {
            screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        }
        if (defaultTexture == null)
        {
            defaultTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        }
        SetMainScreenSetter();
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
        //SetMainScreenSetter();
        //CmdSetClientImage(id);
    }

    public void SetMainScreenSetter()
    {
        if (mainScreenSetter == null)
        {
            mainScreenSetter = GameObject.FindGameObjectWithTag("MainScreenSetter").GetComponent<MainScreenSetter>();
        }
    }

    private void Update()
    {
    }

    public void SendOn(NetworkIdentity targetPlayer)
    {
        if (isLocalPlayer/*targetPlayer == connectionToClient.identity*/)
        {
            // �ڱ� �ڽ��� Ŭ���� ���
            // texture null�� �ٲ������
            Debug.Log("�ڱ��ڽ� Ŭ��");
            InitImage();
        }
        else
        {
            CmdSendCapture(targetPlayer, connectionToClient);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSendCapture(NetworkIdentity targetPlayer, NetworkConnectionToClient reciever = null)
    {
        Debug.Log($"CmdSendCapture ĸó��� {targetPlayer} / ȣ���� �� {reciever}");
        var userImage = targetPlayer.GetComponent<UserImage>();
        //userVideo.RegisterChanel(reciever.identity);
        userImage.TargetStartCapture(connectionToClient, reciever.identity);
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

        yield return waitForEndOfFrame;

        // ȭ�� ĸó�Ͽ� Texture2D�� ����
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        CmdTargetUpdateScreen(targetid, screenTexture.EncodeToJPG());

        Debug.Log("Screen captured and texture updated.");
    }

    [Command]
    private void CmdTargetUpdateScreen(NetworkIdentity reciever, byte[] screenData)
    {
        TargetUpdateScreen(reciever.connectionToClient, screenData);
    }

    [TargetRpc] // Ư�� Ŭ���̾�Ʈ���� ȭ�� ������ ����
    private void TargetUpdateScreen(NetworkConnection targetConnection, byte[] screenData)
    {
        Debug.Log("TargetUpdateScreen");

        if (screenData != null)
        {
            screenTexture.LoadImage(screenData);
            mainScreenSetter.SetMainScreen(screenTexture);
        }
        else
        {
            mainScreenSetter.SetMainScreen(defaultTexture);
        }
    }

    public void SetDefaultSprite(byte[] texture)
    {
        Debug.Log("SetDefaultSprite");
        if(defaultTexture != null)
        {
            defaultTexture.LoadImage(texture);
        }
        InitImage();
    }

    public void InitImage()
    {
        Debug.Log("InitImage");
        Debug.Log(mainScreenSetter);
        SetMainScreenSetter();
        mainScreenSetter.SetMainScreen(defaultTexture);
    }
}
