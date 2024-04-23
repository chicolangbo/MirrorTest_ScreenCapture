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

        if(isLocalPlayer)
        {
            Debug.Log("로컬 어웨이크");
        }
        else
        {
            Debug.Log("아더 어웨이크");
        }

        profileImage = transform.GetChild(0).GetComponent<Image>();
        mainScreen = GameObject.FindGameObjectWithTag("MainScreen").GetComponent<Image>();
        playerName = GameObject.FindGameObjectWithTag("Name").GetComponent<TextMeshProUGUI>();
        contents = GameObject.FindGameObjectWithTag("Contents").GetComponent<Transform>();
        logManager = GameObject.FindGameObjectWithTag("LogManager").GetComponent<LogManager>();

        var sendBtn = GameObject.FindGameObjectWithTag("SendBtn").GetComponent<Button>();
        sendBtn.onClick.AddListener(SetProfileImage);

        // 수정 필요
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
        //SetOtherClientsImage();
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

    //private void OnProfileImageChanged(byte[] oldData, byte[] newData)
    //{
    //    if(!isLocalPlayer && newData != null)
    //    {
    //        Debug.Log("UserCapture : OnProfileImageChanged => true!");
    //        tempTexture.LoadImage(newData);
    //        //var texture = new Texture2D(1, 1);
    //        //texture.LoadImage(newData);
    //        profileImage.sprite = GetImageFromTexture2D(tempTexture);
    //    }
    //    else
    //    {
    //        Debug.Log("UserCapture : OnProfileImageChanged => false!");
    //    }
    //}    

    public void SetProfileImage()
    {
        if (isLocalPlayer)
        {
            Debug.Log($"{gameObject.name} : UserCapture : SetProfileImage");
            tempTexture = CaptureScreen();
            curTextureData = tempTexture.EncodeToJPG();
            CmdUpdateProfileImage(curTextureData);
            //Destroy(tempTexture);
        }
        else
        {
            if(tempTexture == null)
            {
                tempTexture = new Texture2D(1, 1);
            }
            tempTexture.LoadImage(curTextureData);
            profileImage.sprite = GetImageFromTexture2D(tempTexture);
        }
    }

    //public void SetOtherClientsImage()
    //{
    //    if(!isLocalPlayer && curTextureData != null)
    //    {
    //        Debug.Log($"{gameObject.name} : UserCapture : SetOtherClientsImage");
    //        Texture2D texture = new Texture2D(1, 1);
    //        texture.LoadImage(curTextureData);
    //        profileImage.sprite = GetImageFromTexture2D(texture);
    //        Destroy(texture);
    //    }
    //}

    [Command]
    private void CmdUpdateProfileImage(byte[] receivedByte)
    {
        Debug.Log("UserCapture : CmdUpdateProfileImage");

        //curTextureData = receivedByte;
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
            //var texture = new Texture2D(1, 1);
            //texture.LoadImage(curTextureData);
            if(tempTexture == null)
            {
                Debug.Log("텍스처 생성!");
                tempTexture = new Texture2D(1, 1);
            }
            tempTexture.LoadImage(curTextureData);
            Destroy(mainScreen.sprite);
            mainScreen.sprite = GetImageFromTexture2D(tempTexture);
            //Destroy(texture);
        }
    }

    [ClientRpc]
    public void RpcReceiveProfileImage(byte[] receivedByte)
    {
        Debug.Log("UserCapture : RpcReceiveProfileImage");

        //Debug.Log($"curTextureData is null? : {curTextureData == null}");
        //Debug.Log($"tempTexturevvvvvvv is null? : {tempTexture == null}");
        //Debug.Log($"profileImage is null? : {profileImage == null}");
        curTextureData = receivedByte; // 동기화 ㅇ
        if (tempTexture == null)
        {
            Debug.Log("텍스처 생성!");
            tempTexture = new Texture2D(1, 1);
        }
        tempTexture.LoadImage(receivedByte);
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
