using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class FurnitureTestSpawner : MonoBehaviour
{
    public Button button;
    public TMP_Dropdown dropdown;

    private string targetId;

    void Start()
    {
        // 기존 옵션 초기화
        dropdown.ClearOptions();

        // 새로운 옵션 추가
        List<string> options = new List<string>();
        foreach(var furniture in FurnitureManager.Instance.furnitureDatas)
        {
            options.Add(furniture.id);
        }

        dropdown.AddOptions(options);

        // 선택 시 이벤트
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        button.onClick.AddListener(Spawn);

        if (dropdown.options.Count > 0)
        {
            dropdown.value = 0;
            targetId = dropdown.options[0].text;
        }
    }

    public void Spawn()
    {
        if(!string.IsNullOrEmpty(targetId))
        {
            FurnitureManager.Instance.GetFurniture(targetId);
        }
        else
        {
            Debug.Log("목표 없음");
        }
    }

    void OnDropdownValueChanged(int index)
    {
        Debug.Log("선택된 옵션: " + dropdown.options[index].text);
        targetId = dropdown.options[index].text;
    }
}
