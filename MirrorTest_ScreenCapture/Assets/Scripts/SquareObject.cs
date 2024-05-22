using UnityEngine;
using System.Collections;
using TMPro;

public class ToggleSizeSetter : MonoBehaviour
{
    public float offset;
    private RectTransform rectTransform;
    public RectTransform toggleRectTransform;
    public RectTransform label;

    void Start()
    {
        // StartCoroutine를 사용하여 레이아웃이 적용된 후 사이즈를 조정
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(AdjustWidthToHeight());
    }

    private IEnumerator AdjustWidthToHeight()
    {
        // 1 프레임 대기하여 레이아웃이 적용되도록 함
        yield return null;

        // 높이를 기반으로 가로 크기 설정
        var height = rectTransform.rect.height;
        toggleRectTransform.sizeDelta = new Vector2(height - offset * 2, height - offset * 2);
        toggleRectTransform.localPosition = new Vector2(offset, -offset);

        // 라벨
        var labelWidth = rectTransform.rect.width - height;

        label.sizeDelta = new Vector2(labelWidth, height);
        label.localPosition = new Vector2(height, 0);
    }
}
