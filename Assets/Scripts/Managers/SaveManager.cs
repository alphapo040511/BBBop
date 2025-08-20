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
            _saveData.ownedFurnitures.Add(owned);        // �������� ���� ����
        }

        foreach (var placed in FurnitureManager.Instance.PlacedFurnitures.Values)
        {
            _saveData.placedFurnituresData.Add(placed);  // ��ġ���� ���� ����
        }

        string json = JsonUtility.ToJson(_saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"���� �Ϸ�: {savePath}");
    }

    public void LoadData()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("���̺� ���� ����");
            _saveData = new SaveData();
            return;
        }

        string json = File.ReadAllText(savePath);
        _saveData = JsonUtility.FromJson<SaveData>(json);
        Debug.Log("�ε� �Ϸ�");

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
