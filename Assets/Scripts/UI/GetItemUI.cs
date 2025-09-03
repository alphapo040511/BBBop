using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FurnitureData;
using UnityEngine.UI;

public class GetItemUI : MonoBehaviour
{
    [Header("UI Setting")]
    public Image icon;
    public TextMeshProUGUI furnitureName;
    public TextMeshProUGUI count;

    [Header("Color Setting")]
    public Color commonColor = Color.gray;
    public Color rareColor = Color.cyan;
    public Color epicColor = Color.magenta;
    public Color uniqueColor = Color.yellow;

    public void InitializeUI(FurnitureData data, int amount)
    {
        OwnedFurniture item = FurnitureManager.Instance.FindOwnedDate(data.id);

        this.icon.sprite = data.icon;
        SetColor(data.probability);

        // 텍스트 적용
        furnitureName.text = data.itemName;
        count.text = $"X{amount}";
    }

    private void SetColor(Probability probability)
    {
        Image image = GetComponent<Image>();
        Color color = Color.gray;
        switch (probability)
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
