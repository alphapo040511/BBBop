using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OwnedFurnitureUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI furnitureName;
    public TextMeshProUGUI count;
    public TextMeshProUGUI content;

    private OwnedFurniture ownedFurniture;
    private SimplePlacer placer;

    private void OnDestroy()
    {
        ownedFurniture.onChangeCount -= UpdateUI;                 // ������ ��� Ȯ�� �� �̺�Ʈ ����
    }

    public void Select()
    {
        placer.SelectFurnitureChange(FurnitureManager.Instance.FindFurnitureDate(ownedFurniture.furnitureId));
    }

    public void InitializeUI(FurnitureData data, SimplePlacer placer)
    {
        OwnedFurniture item = FurnitureManager.Instance.FindOwnedDate(data.id);

        ownedFurniture = item;

        furnitureName.text = item.furnitureId;
        this.icon.sprite = data.icon;
        count.text = $"X{item.count}";
        content.text = $"{data.intervalTime}Sec {data.goldAmount}G";
        this.placer = placer;

        gameObject.SetActive(item.count > 0);

        item.onChangeCount += UpdateUI;                     // ������ ��� Ȯ�� �� �̺�Ʈ ���
    }

    public void UpdateUI(int count)
    {
        this.count.text = $"X{count}";
        Debug.Log(ownedFurniture.furnitureId + ":" +count+"��");
        gameObject.SetActive(count > 0);                    // ������ ��쿡�� ����
    }
}
