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
        ownedFurniture.onChangeCount -= UpdateUI;                 // 아이템 사용 확인 용 이벤트 해제
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

        item.onChangeCount += UpdateUI;                     // 아이템 사용 확인 용 이벤트 등록
    }

    public void UpdateUI(int count)
    {
        this.count.text = $"X{count}";
        Debug.Log(ownedFurniture.furnitureId + ":" +count+"개");
        gameObject.SetActive(count > 0);                    // 보유한 경우에만 가능
    }
}
