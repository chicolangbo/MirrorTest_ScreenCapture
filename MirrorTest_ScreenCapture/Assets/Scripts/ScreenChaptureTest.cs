using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MainScreenSetter : MonoBehaviour
{
    public RawImage mainScreen;
    public RectTransform mainPanel;
    public float offset;

    private float maxWidth;
    private float maxHeight;
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private Texture2D screenTexture;

    private void Start()
    {
        StartCoroutine(Init());
    }

    private void Update()
    {
        //if(Input.GetMouseButtonDown(0))
        //{
        //    StartCoroutine(CaptureScreenByReadPixels());
        //}
    }

    public void SetMainScreenSize(float heightRatio)
    {
        var mainScreenRect = mainScreen.GetComponent<RectTransform>();

        var height = maxWidth * (1 / heightRatio);
        if(height > maxHeight)
        {
            height = maxHeight;
            mainScreenRect.sizeDelta = new Vector2(height * heightRatio, height);
        }
        else
        {
            mainScreenRect.sizeDelta = new Vector2(maxWidth, maxWidth * (1 / heightRatio));
        }
    }

    public void SetMainScreen(Texture2D newTexture)
    {
        Debug.Log("SetMainScreen");

        mainScreen.texture = newTexture;
    }

    public IEnumerator CaptureScreenByReadPixels()
    {
        while (true)
        {
            yield return waitForEndOfFrame;

            // 화면 캡처하여 Texture2D로 저장
            screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenTexture.Apply();

            // UI에 텍스처 설정
            mainScreen.texture = screenTexture;

            // Debug 로그 출력
            Debug.Log("Screen captured and texture updated.");
        }
    }

    // UI 전용 카메라가 있어야 가능
    //public IEnumerator CaptureScreenByRequest()
    //{
    //    while(true)
    //    {
    //        yield return waitForEndOfFrame;

    //        renderCamera.targetTexture = renderTexture;
    //        renderCamera.Render();

    //        AsyncGPUReadback.Request(renderTexture, 0, request => {
    //            if (request.hasError)
    //            {
    //                Debug.LogError("GPU readback error detected.");
    //                return;
    //            }
    //            screenTexture.LoadRawTextureData(request.GetData<byte>());
    //            screenTexture.Apply();
    //            mainScreen.texture = screenTexture;
    //        });

    //        Camera.main.targetTexture = null;
    //        Debug.Log("CaptureScreenByRequest");
    //    }
    //}


    private void OnDestroy()
    {
        if (screenTexture != null)
        {
            Destroy(screenTexture);
        }
    }

    public IEnumerator Init()
    {
        screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        yield return waitForEndOfFrame;

        maxWidth = mainPanel.rect.width - 2 * offset;
        maxHeight = mainPanel.rect.height;

        float screenHeight = 0;
        float screenWidth = 0;
        float heightRatio = 0;

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            screenHeight = Display.main.systemHeight;
            screenWidth = Display.main.systemWidth;
            heightRatio = screenWidth / screenHeight;
        }
        else
        {
            screenHeight = Screen.safeArea.height;
            screenWidth = Screen.safeArea.width;
            heightRatio = screenWidth / screenHeight;
        }

        SetMainScreenSize(heightRatio);
    }
}
