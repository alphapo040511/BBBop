using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;

public class SaveData
{
    public string gold;
    public List<OwnedFurniture> ownedFurnitures = new List<OwnedFurniture>();
    public List<PlacedFurnitureData> placedFurnituresData = new List<PlacedFurnitureData>();
}

public class SaveManager : SingletonMonoBehaviour<SaveManager>
{
    private string savePath => Application.persistentDataPath + "/save.json";
    public SaveData _saveData { get; private set; }

    private float autoSaveInterval = 15f;
    private float timer;

    void Start()
    {
        LoadData();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= autoSaveInterval)
        {
            SaveData();
            timer = 0f;
            Debug.Log("자동저장");
        }
    }

    public void SaveData()
    {
        _saveData = new SaveData();

        _saveData.gold = ResourceManager.Instance.gold.ToString(); // 보유중인 G 저장

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

            _saveData.ownedFurnitures = new List<OwnedFurniture>()                      // 초기 보유 가구 생성
            {
                new OwnedFurniture ("Conveyor", 7)
            };

            _saveData.placedFurnituresData = new List<PlacedFurnitureData>() {          // 초기 설치 가구 생성
                 new PlacedFurnitureData ("POS", new Vector2Int(1, 1), 0),
                 new PlacedFurnitureData("Conveyor", new Vector2Int(1, 2), 0),
                new PlacedFurnitureData("Capsule_Default", new Vector2Int(1, 3), 180)
                };
        }
        else
        {
            string json = File.ReadAllText(savePath);
            _saveData = JsonUtility.FromJson<SaveData>(json);
        }

        Debug.Log("로드 완료");

        FurnitureManager.Instance.LoadOwnedData(_saveData.ownedFurnitures);
    }

    public void LoadPlacedData(GridManager gridManager)
    { 
        LoadData();
        gridManager.InstantiateFormSave(_saveData.placedFurnituresData);
    }
}
