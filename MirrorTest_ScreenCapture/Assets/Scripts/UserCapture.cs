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
    private TextMeshProUGUI playerName;

    private void Awake()
    {
        Debug.Log("UserCapture : Awake");
        profileImage = GetComponent<Image>();
        mainScreen = GameObject.FindGameObjectWithTag("MainScreen").GetComponent<Image>();
        playerName = GameObject.FindGameObjectWithTag("Name").GetComponent<TextMeshProUGUI>();
        GameObject.FindGameObjectWithTag("SendBtn").GetComponent<Button>().onClick.AddListener(SetProfileImage);


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
        SetProfileImage();
        SetMainScreen();
    }

    // 스샷 함수 for user
    public Texture2D CaptureScreen()
    {
        return ScreenCapture.CaptureScreenshotAsTexture();
    }

    // 내 사진 업데이트 for user, other clients
    public void SetProfileImage()
    {
        if(isLocalPlayer)
        {
            // 프로필 이미지를 모든 클라이언트에게 동기화
            //Debug.Log($"{gameObject.name} : UserCapture : SetProfileImage");
            var texture = CaptureScreen();
            curTexture = texture;
            profileImage.sprite = GetImageFromTexture2D(texture);
            CmdUpdateProfileImage(texture);
            //ProfileManager.instance.CmdAddAndUpdateProfiles(gameObject.GetComponent<RectTransform>(), texture);
        }
        else
        {
            Debug.Log($"UserCapture : cur texture is null? {curTexture == null}");
            if (curTexture != null)
            {
                profileImage.sprite = GetImageFromTexture2D(curTexture);
            }
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
        Debug.Log($"main screen is null? : {mainScreen == null}");
        Debug.Log($"texture is null? : {curTexture == null}");
        if(curTexture != null)
        {
            mainScreen.sprite = GetImageFromTexture2D(curTexture);
        }
    }

    [ClientRpc]
    public void RpcReceiveProfileImage(Texture2D texture)
    {
        Debug.Log("UserCapture : RpcReceiveProfileImage");
        profileImage.sprite = GetImageFromTexture2D(texture);
    }
}
