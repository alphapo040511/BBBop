using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditModeUI : MonoBehaviour
{
    public OwnedFurnitureUI UIPrefab;
    public Transform content;                   // UI 를 생성할 Transform

    public SimplePlacer placer;

    void Start()
    {
        foreach(var furniture in FurnitureManager.Instance.furnitureDatas)
        {
            CreatUI(furniture);
        }
    }

    public void ResetUI()
    {
        foreach(Transform ui in content)
        {
            Destroy(ui.gameObject);
        }

        foreach (var furniture in FurnitureManager.Instance.furnitureDatas)
        {
            CreatUI(furniture);
        }
    }

    private void CreatUI(FurnitureData data)
    {
        if (UIPrefab == null || content == null) return;

        OwnedFurnitureUI ui = Instantiate(UIPrefab, content);
        ui.InitializeUI(data, placer);
    }
}
