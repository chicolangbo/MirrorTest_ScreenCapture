using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;
//using static UnityEditor.Timeline.TimelinePlaybackControls;
//using System.Runtime.CompilerServices;

public class UserCapture : NetworkBehaviour
{
    [SyncVar/*(hook = nameof(OnProfileImageChanged))*/]
    private byte[] curTextureData;

    public LogManager logManager;

    private Texture2D tempTexture;
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

        var sendBtn = GameObject.FindGameObjectWithTag("SendBtn").GetComponent<Button>();
        sendBtn.onClick.AddListener(SetProfileImage);

        // 수정 필요
        var num = NetworkServer.connections.Count;
        playerName.text = "client" + num;
    }

    private void Start()
    {
        Debug.Log("UserCapture : Start");
        SetParent();
        SetProfileImage();

        if (isLocalPlayer)
        {
            SetMainScreen();
        }
    }

    public Texture2D CaptureScreen()
    {
        Debug.Log("UserCapture : CaptureScreen");
        return ScreenCapture.CaptureScreenshotAsTexture();
    }  

    public void SetProfileImage()
    {
        if (isLocalPlayer)
        {
            Debug.Log($"{gameObject.name} : UserCapture : SetProfileImage");
            tempTexture = CaptureScreen();
            GC.Collect();
            curTextureData = tempTexture.EncodeToJPG();
            CmdUpdateProfileImage(curTextureData);
        }
        else
        {
            if(tempTexture == null)
            {
                tempTexture = new Texture2D(1, 1);
            }
            tempTexture.LoadImage(curTextureData);
            GC.Collect(); // test
            profileImage.sprite = GetImageFromTexture2D(tempTexture);
        }
    }

    [Command]
    private void CmdUpdateProfileImage(byte[] receivedByte)
    {
        Debug.Log("UserCapture : CmdUpdateProfileImage");

        RpcReceiveProfileImage(receivedByte);
    }

    public Sprite GetImageFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void SetMainScreen()
    {
        Debug.Log("UserCapture : SetMainScreen");

        if(curTextureData != null)
        {
            if(tempTexture == null)
            {
                Debug.Log("텍스처 생성!");
                tempTexture = new Texture2D(1, 1);
            }
            tempTexture.LoadImage(curTextureData);
            GC.Collect();
            Destroy(mainScreen.sprite);
            mainScreen.sprite = GetImageFromTexture2D(tempTexture);
        }
    }

    [ClientRpc]
    public void RpcReceiveProfileImage(byte[] receivedByte)
    {
        Debug.Log("UserCapture : RpcReceiveProfileImage");
        curTextureData = receivedByte;
        if (tempTexture == null)
        {
            Debug.Log("텍스처 생성!");
            tempTexture = new Texture2D(1, 1);
        }
        tempTexture.LoadImage(receivedByte);
        GC.Collect();
        profileImage.sprite = GetImageFromTexture2D(tempTexture);
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

        // 자식도 맞춰줘야 함
        var childRect = rect.GetChild(0).GetComponent<RectTransform>();
        childRect.sizeDelta = Utils.GetChildRectAdjusted(rect.sizeDelta, 10f);
    }
}
