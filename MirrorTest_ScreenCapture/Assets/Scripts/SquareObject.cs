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
        // StartCoroutine�� ����Ͽ� ���̾ƿ��� ����� �� ����� ����
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(AdjustWidthToHeight());
    }

    private IEnumerator AdjustWidthToHeight()
    {
        // 1 ������ ����Ͽ� ���̾ƿ��� ����ǵ��� ��
        yield return null;

        // ���̸� ������� ���� ũ�� ����
        var height = rectTransform.rect.height;
        toggleRectTransform.sizeDelta = new Vector2(height - offset * 2, height - offset * 2);
        toggleRectTransform.localPosition = new Vector2(offset, -offset);

        // ��
        var labelWidth = rectTransform.rect.width - height;

        label.sizeDelta = new Vector2(labelWidth, height);
        label.localPosition = new Vector2(height, 0);
    }
}
