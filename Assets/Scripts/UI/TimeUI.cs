using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public Image image;
    private RectTransform uiElement;
    public float speed = 100f; // 픽셀/초
    public float targetDistance = 100f;
    private float movedDistance = 0f;

    private Vector2 initPosition;


    private void Awake()
    {
        uiElement = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // UI 색상 변경
        Color color = tmp.color;
        color.a = 1f;
        tmp.color = color;
    }

    public void Show(int time, Vector2 position)
    {
        if (tmp != null)
        {
            tmp.text = $"-{time}%";

            // UI 색상 변경
            Color color = tmp.color;
            color.a = 1f;
            tmp.color = color;
            image.color = color;
        }

        initPosition = position;
        uiElement.anchoredPosition = position;

        movedDistance = 0;
    }

    void Update()
    {
        if (uiElement == null) return;

        Color color = tmp.color;

        if (movedDistance >= targetDistance)
        {
            // 비활성화
            ObjectPool.Instance.Despawn("TimeUI", gameObject);
            return;
        }

        movedDistance += speed * Time.deltaTime;

        uiElement.anchoredPosition = initPosition + Vector2.up * movedDistance;

        // UI 색상 변경
        color.a = 1 - (movedDistance / targetDistance);
        tmp.color = color;
        image.color = color;
    }
}
