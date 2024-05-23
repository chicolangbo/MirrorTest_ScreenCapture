using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCaptureTest : MonoBehaviour
{
    public RawImage mainScreen;
    public RectTransform mainPanel;
    public float offset;

    private float maxWidth;
    private float maxHeight;
    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private RenderTexture renderTexture;
    private Texture2D screenTexture;

    private void Start()
    {
        StartCoroutine(Init());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(CaptureScreen());
        }
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

    public IEnumerator CaptureScreen()
    {
        while (true)
        {
            yield return waitForEndOfFrame;

            // ȭ�� ĸó�Ͽ� Texture2D�� ����
            screenTexture = ScreenCapture.CaptureScreenshotAsTexture();

            // UI�� �ؽ�ó ����
            mainScreen.texture = screenTexture;

            // Debug �α� ���
            Debug.Log("Screen captured and texture updated.");
        }
    }

    private void OnDestroy()
    {
        if (screenTexture != null)
        {
            Destroy(screenTexture);
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }

    public IEnumerator Init()
    {
        yield return waitForEndOfFrame;

        Debug.Log($"maxWidth : {mainPanel.rect.width}");
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
        Debug.Log(heightRatio);

        // RenderTexture �� Texture2D �ʱ�ȭ
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        Debug.Log("RenderTexture and Texture2D initialized.");
    }
}
