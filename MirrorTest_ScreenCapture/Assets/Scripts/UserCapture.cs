using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class UserCapture : NetworkBehaviour
{
    [SyncVar]
    private Texture2D curTexture;

    private Image profileImage;
    private Image mainScreen;
    private TextMeshProUGUI name;

    private void Awake()
    {
        Debug.Log("UserCapture : Awake");
        profileImage = GetComponent<Image>();
        mainScreen = GameObject.FindGameObjectWithTag("MainScreen").GetComponent<Image>();
        name = GameObject.FindGameObjectWithTag("Name").GetComponent<TextMeshProUGUI>();
        GameObject.FindGameObjectWithTag("SendBtn").GetComponent<Button>().onClick.AddListener(SetProfileImage);
    }

    private void Start()
    {
        Debug.Log("UserCapture : Start");
        name.text = NewNetworkManager.singleton.name;
        SetProfileImage();
        SetMainScreen();
    }

    // ���� �Լ� for user
    public Texture2D CaptureScreen()
    {
        return ScreenCapture.CaptureScreenshotAsTexture();
    }

    // �� ���� ������Ʈ for user, other clients
    public void SetProfileImage()
    {
        if(isLocalPlayer)
        {
            Debug.Log("UserCapture : SetProfileImage");
            var texture = CaptureScreen();
            curTexture = texture;
            profileImage.sprite = GetImageFromTexture2D(texture);

            // ������ �̹����� ��� Ŭ���̾�Ʈ���� ����ȭ
            CmdUpdateProfileImage(texture);
        }
    }

    [Command]
    private void CmdUpdateProfileImage(Texture2D texture)
    {
        Debug.Log("UserCapture : CmdUpdateProfileImage");
        
        curTexture = texture;
        //profileImage.sprite = GetImageFromTexture2D(texture);
        RpcReceiveProfileImage(texture);
    }

    public Sprite GetImageFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void SetMainScreen()
    {
        Debug.Log("UserCapture : SetMainScreen");
        mainScreen.sprite = GetImageFromTexture2D(curTexture);
    }

    [ClientRpc]
    public void RpcReceiveProfileImage(Texture2D texture)
    {
        // ���� �ƴϾ, Receive�ؼ� �ݿ��ؾ� �ϴϱ� isLocalPlayer ����
        //if(isLocalPlayer || isServer)
        //{
            Debug.Log("UserCapture : RpcReceiveProfileImage");
            profileImage.sprite = GetImageFromTexture2D(texture);
        //}
    }
}
