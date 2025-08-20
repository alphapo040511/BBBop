using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData
{
    public long gold;
    public List<OwnedFurniture> ownedFurnitures = new List<OwnedFurniture>();
    public List<PlacedFurnitureData> placedFurnituresData = new List<PlacedFurnitureData>();
}

public class SaveManager : SingletonMonoBehaviour<SaveManager>
{
    private string savePath => Application.persistentDataPath + "/save.json";
    private SaveData _saveData;

    void Start()
    {
        LoadData();
    }

    public void SaveData()
    {
        _saveData = new SaveData();

        foreach (var owned in FurnitureManager.Instance.OwnedFurnitures.Values)
        {
            _saveData.ownedFurnitures.Add(owned);        // 보유중인 가구 저장
        }

        foreach (var placed in FurnitureManager.Instance.PlacedFurnitures.Values)
        {
            _saveData.placedFurnituresData.Add(placed);  // 설치중인 가구 저장
        }

        string json = JsonUtility.ToJson(_saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"저장 완료: {savePath}");
    }

    public void LoadData()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("세이브 파일 없음");
            _saveData = new SaveData();
            return;
        }

        string json = File.ReadAllText(savePath);
        _saveData = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("로드 완료");

        FurnitureManager.Instance.LoadOwnedData(_saveData.ownedFurnitures);
    }

    public void LoadPlacedData(GridManager gridManager)
    {
        if(_saveData == null)
        {
            LoadData();
        }

        gridManager.InstantiateFormSave(_saveData.placedFurnituresData);
    }
}
