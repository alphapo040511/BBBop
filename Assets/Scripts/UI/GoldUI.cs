using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEngine.GraphicsBuffer;

public class GoldUI : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private RectTransform uiElement;
    public float speed = 100f; // 픽셀/초
    public float targetDistance = 100f;
    private float movedDistance = 0f;

    private Vector2 initPosition;
    private Transform targetFurniture;


    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        uiElement = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // UI 색상 변경
        Color color = tmp.color;
        color.a = 1f;
        tmp.color = color;
    }

    public void Show(int gold, Transform target)
    {
        if(tmp != null)
        {
            tmp.text = "+" + gold.ToString("N0") + 'G';

            // UI 색상 변경
            Color color = tmp.color;
            color.a = 1f;
            tmp.color = color;
        }

        targetFurniture = target;

        Vector2 position = Camera.main.WorldToScreenPoint(targetFurniture.position);
        position.y += 50f;
        uiElement.anchoredPosition = position;

        movedDistance = 0;
    }

    void Update()
    {
        if (uiElement == null || targetFurniture == null) return;

        Color color = tmp.color;

        if (movedDistance >= targetDistance)
        {
            // 비활성화
            ObjectPool.Instance.Despawn("GoldUI", gameObject);
            return;
        }

        Vector2 position = Camera.main.WorldToScreenPoint(targetFurniture.position);
        position.y += 50f;

        movedDistance += speed * Time.deltaTime;

        uiElement.anchoredPosition = position + Vector2.up * movedDistance;

        // UI 색상 변경
        color.a = 1 - (movedDistance / targetDistance);
        tmp.color = color;
    }
}
