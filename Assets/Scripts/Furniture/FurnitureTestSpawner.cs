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
        // ���� �ɼ� �ʱ�ȭ
        dropdown.ClearOptions();

        // ���ο� �ɼ� �߰�
        List<string> options = new List<string>();
        foreach(var furniture in FurnitureManager.Instance.furnitureDatas)
        {
            options.Add(furniture.id);
        }

        dropdown.AddOptions(options);

        // ���� �� �̺�Ʈ
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
            Debug.Log("��ǥ ����");
        }
    }

    void OnDropdownValueChanged(int index)
    {
        Debug.Log("���õ� �ɼ�: " + dropdown.options[index].text);
        targetId = dropdown.options[index].text;
    }
}
