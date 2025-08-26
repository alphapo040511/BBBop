using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FurnitureData;

public class OwnedFurnitureUI : MonoBehaviour
{
    [Header("UI Setting")]
    public Image icon;
    public TextMeshProUGUI furnitureName;
    public TextMeshProUGUI count;
    public TextMeshProUGUI content;

    [Header("Color Setting")]
    public Color commonColor = Color.gray;
    public Color rareColor = Color.cyan;
    public Color epicColor = Color.magenta;
    public Color uniqueColor = Color.yellow;

    private OwnedFurniture ownedFurniture;
    private SimplePlacer placer;

    private void OnDestroy()
    {
        ownedFurniture.onChangeCount -= UpdateUI;                 // ������ ��� Ȯ�� �� �̺�Ʈ ����
    }

    public void Select()
    {
        placer.SelectFurnitureChange(FurnitureManager.Instance.FindFurnitureDate(ownedFurniture.furnitureId));
        placer.currentMode = SimplePlacer.PlaceMode.Edit;
    }

    public void InitializeUI(FurnitureData data, SimplePlacer placer)
    {
        OwnedFurniture item = FurnitureManager.Instance.FindOwnedDate(data.id);

        ownedFurniture = item;
        this.placer = placer;

        this.icon.sprite = data.icon;
        SetColor(data.probability);

        // �ؽ�Ʈ ����
        furnitureName.text = data.itemName;
        count.text = $"X{item.count}";
        if(data.canProduceResource)
            content.text = $"{data.intervalTime}Sec {data.goldAmount}G";
        else
            content.enabled = false;

        item.onChangeCount += UpdateUI;                     // ������ ��� Ȯ�� �� �̺�Ʈ ���

        gameObject.SetActive(item.count > 0);
    }

    public void UpdateUI(int count)
    {
        this.count.text = $"X{count}";
        Debug.Log(ownedFurniture.furnitureId + ":" +count+"��");
        gameObject.SetActive(count > 0);                    // ������ ��쿡�� ����
    }

    private void SetColor(Probability probability)
    {
        Image image = GetComponent<Image>();
        Color color = Color.gray;
        switch(probability)
        {
            case Probability.Common:
                color = commonColor;
                break;
            case Probability.Rare:
                color = rareColor;
                break;
            case Probability.Epic:
                color = epicColor;
                break;
            case Probability.Unique:
                color = uniqueColor;
                break;
        }

        image.color = color;
    }
}
