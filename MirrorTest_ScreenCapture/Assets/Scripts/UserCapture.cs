using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UserCapture : NetworkBehaviour
{
    //[SyncVar]
    private Image profileImage;
    private Image mainScreen;

    private Texture2D curTexture;

    private void Awake()
    {
        Debug.Log("UserCapture : Awake");
        profileImage = GetComponent<Image>();
        mainScreen = GameObject.FindGameObjectWithTag("MainScreen").GetComponent<Image>();
        GameObject.FindGameObjectWithTag("SendBtn").GetComponent<Button>().onClick.AddListener(SetProfileImage);
    }

    private void Start()
    {
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
        Debug.Log("SetProfileImage");
        var texture = CaptureScreen();
        curTexture = texture;
        profileImage.sprite = GetImageFromTexture2D(texture);

        // 프로필 이미지를 모든 클라이언트에게 동기화
        CmdUpdateProfileImage(texture);
    }

    [Command]
    private void CmdUpdateProfileImage(Texture2D texture)
    {
        Debug.Log("CmdUpdateProfileImage");
        profileImage.sprite = GetImageFromTexture2D(texture);
        RpcReceiveProfileImage(texture);
    }

    public Sprite GetImageFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void SetMainScreen()
    {
        mainScreen.sprite = GetImageFromTexture2D(curTexture);
    }

    [ClientRpc(includeOwner = false)]
    public void RpcReceiveProfileImage(Texture2D texture)
    {
        profileImage.sprite = GetImageFromTexture2D(texture);
    }
}
