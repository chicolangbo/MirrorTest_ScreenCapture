using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
//using static UnityEditor.Timeline.TimelinePlaybackControls;
//using System.Runtime.CompilerServices;

public class UserCapture : NetworkBehaviour
{
    [SyncVar]
    private Texture2D curTexture;

    public LogManager logManager;

    private Image profileImage;
    private Image mainScreen;
    private TextMeshProUGUI playerName;

    private Transform contents;


    private void Awake()
    {
        Debug.Log("UserCapture : Awake");
        profileImage = transform.GetChild(0).GetComponent<Image>();
        mainScreen = GameObject.FindGameObjectWithTag("MainScreen").GetComponent<Image>();
        playerName = GameObject.FindGameObjectWithTag("Name").GetComponent<TextMeshProUGUI>();
        contents = GameObject.FindGameObjectWithTag("Contents").GetComponent<Transform>();
        logManager = GameObject.FindGameObjectWithTag("LogManager").GetComponent<LogManager>();
        GameObject.FindGameObjectWithTag("SendBtn").GetComponent<Button>().onClick.AddListener(SetProfileImage);

        // ���� �ʿ�
        var num = NetworkServer.connections.Count;
        if (num <= 0)
        {
            playerName.text = "host";
        }
        else
        {
            playerName.text = "client" + num;
        }
    }

    private void Start()
    {
        Debug.Log("UserCapture : Start");
        SetParent();
        SetProfileImage();
        SetOtherClientsImage();
        SetMainScreen();
    }

    // ���� �Լ� for user
    public Texture2D CaptureScreen()
    {
        Debug.Log("UserCapture : CaptureScreen");
        return ScreenCapture.CaptureScreenshotAsTexture();
    }

    // �� ���� ������Ʈ for user, other clients
    public void SetProfileImage()
    {
        if (isLocalPlayer)
        {
            // ������ �̹����� ��� Ŭ���̾�Ʈ���� ����ȭ
            Debug.Log($"{gameObject.name} : UserCapture : SetProfileImage");
            var texture = CaptureScreen();
            var textureToByte = texture.EncodeToJPG();
            //profileImage.sprite = GetImageFromTexture2D(texture);

            //curTexture = texture;

            CmdUpdateProfileImage(textureToByte);
            //ProfileManager.instance.CmdAddAndUpdateProfiles(gameObject.GetComponent<RectTransform>(), texture);
        }
    }

    public void SetOtherClientsImage()
    {
        if(!isLocalPlayer && curTexture != null)
        {
            profileImage.sprite = GetImageFromTexture2D(curTexture);
        }
    }

    [Command]
    private void CmdUpdateProfileImage(/*Texture2D texture*/ byte[] receivedByte)
    {
        Debug.Log("UserCapture : CmdUpdateProfileImage");
        
        // origin code
        //curTexture = texture;
        ////profileImage.sprite = GetImageFromTexture2D(texture);
        //var textureToByte = texture.EncodeToPNG();
        //RpcReceiveProfileImage(textureToByte);

        // new code
        RpcReceiveProfileImage(receivedByte);

        //var texture = new Texture2D(1, 1);
        //texture.LoadImage(receivedByte);
        //curTexture = texture;
        //profileImage.sprite = GetImageFromTexture2D(texture);
        //Debug.Log(profileImage.sprite.texture);
    }

    public Sprite GetImageFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void SetMainScreen()
    {
        Debug.Log("UserCapture : SetMainScreen");
        if(curTexture != null)
        {
            mainScreen.sprite = GetImageFromTexture2D(curTexture);
        }
    }

    [ClientRpc]
    public void RpcReceiveProfileImage(/*Texture2D texture*/ byte[] receivedByte)
    {
        // origin code working in editor mode
        Debug.Log("UserCapture : RpcReceiveProfileImage");
        //profileImage.sprite = GetImageFromTexture2D(texture);

        var texture = new Texture2D(1, 1);
        texture.LoadImage(receivedByte);

        curTexture = texture;
        //if(curTexture == null)
        //{
        //}

        profileImage.sprite = GetImageFromTexture2D(texture);
    }

    public void SetParent()
    {
        transform.SetParent(contents);
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = UIManager.Instance.ClientProfileSize;
        rect.localScale = new Vector3(1, 1, 1);
        var tempPos = rect.position;
        tempPos.z = 0f;
        rect.localPosition = tempPos;

        // �ڽĵ� ������� ��
        var childRect = rect.GetChild(0).GetComponent<RectTransform>();
        childRect.sizeDelta = Utils.GetChildRectAdjusted(rect.sizeDelta, 10f);
    }
}
