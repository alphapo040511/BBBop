using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

// ����� class
[System.Serializable]
public class OwnedFurniture
{
    public Action<int> onChangeCount;

    public string furnitureId;
    public int count;  // ���� ���� ����

    public OwnedFurniture(string furnitureId, int count)
    {
        this.furnitureId = furnitureId;
        this.count = count;
    }
}

// ��ġ�� ����
[System.Serializable]
public class PlacedFurnitureData
{
    public string id;
    public Vector2Int start;
    public int rotation;

    public PlacedFurnitureData(string id, Vector2Int start, int rotation)
    {
        this.id = id;
        this.start = start;
        this.rotation = rotation;
    }
}

public class FurnitureManager : SingletonMonoBehaviour<FurnitureManager>
{
    public List<FurnitureData> furnitureDatas = new List<FurnitureData>();       // �ӽ� ���� ���� ����Ʈ

    private Dictionary<string, OwnedFurniture> ownedFurnitures = new Dictionary<string, OwnedFurniture>();                          // ���� ����
    private Dictionary<Vector2Int, PlacedFurnitureData> placedFurnitures = new Dictionary<Vector2Int, PlacedFurnitureData>();       // ��ġ�� ����


    // �ܺ� �б� ���� Dictionary
    public ReadOnlyDictionary<string, OwnedFurniture> OwnedFurnitures => new ReadOnlyDictionary<string, OwnedFurniture>(ownedFurnitures);
    public ReadOnlyDictionary<Vector2Int, PlacedFurnitureData> PlacedFurnitures => new ReadOnlyDictionary<Vector2Int, PlacedFurnitureData>(placedFurnitures);


    // ������ ȹ��
    public void GetFurniture(string id, int count = 1)
    {
        if(ownedFurnitures.ContainsKey(id))
        {
            ownedFurnitures[id].count += count;
        }
        else
        {
            ownedFurnitures.Add(id, new OwnedFurniture(id, count));
        }

        ownedFurnitures[id].onChangeCount?.Invoke(ownedFurnitures[id].count);
        Debug.Log($"(ID:{id}) ���� ���� : {ownedFurnitures[id].count}");
    }

    // ���� ���
    public bool UseFurniture(string id)
    {
        if (ownedFurnitures.ContainsKey(id))
        {
            ownedFurnitures[id].count--;
            ownedFurnitures[id].onChangeCount?.Invoke(ownedFurnitures[id].count);
            Debug.Log($"(ID:{id}) ���� ���� : {ownedFurnitures[id].count}");

            if (ownedFurnitures[id].count <= 0)
            {
                ownedFurnitures.Remove(id);
            }

            return true;
        }
        else
        {
            Debug.LogWarning($"ID:{id} ������ �����ϰ� ���� �ʽ��ϴ�.");
            return false;
        }
    }

    public bool CanUseFurniture(string id)
    {
        if (ownedFurnitures.ContainsKey(id))
        {
            return ownedFurnitures[id].count > 0;
        }

        return false;
    }

    // ���� ��ġ
    public bool PlaceFurniture(PlacedFurnitureData data)
    {
        if (placedFurnitures.ContainsKey(data.start))
        {
            Debug.LogWarning($"{data.start} ��ġ�� �̹� ��ġ�� ������ �ֽ��ϴ�.");
            return false;
        }
        else
        {
            placedFurnitures[data.start] = data;
            return true;
        }
    }

    public bool UnplaceFurniture(Vector2Int start)
    {
        if(placedFurnitures.ContainsKey(start))
        {
            GetFurniture(placedFurnitures[start].id);

            placedFurnitures.Remove(start);

            return true;
        }

        return false;
    }

    public void LoadOwnedData(List<OwnedFurniture> ownedFurnitures)
    {
        this.ownedFurnitures.Clear();
        foreach (var owned in ownedFurnitures)
        {
            GetFurniture(owned.furnitureId, owned.count);
        }
    }


    public void PlaceFurnitureClear()
    {
        placedFurnitures.Clear();
    }

    public FurnitureData FindFurnitureDate(string id)
    {
        FurnitureData temp = furnitureDatas.Find(x => x.id == id);
        if(temp == null)
        {
            return null;
        }

        return temp;
    }

    public OwnedFurniture FindOwnedDate(string id)
    {
        if(ownedFurnitures.ContainsKey(id))
        {
            return null;
        }


        return ownedFurnitures[id];
    }

    void OnGUI()
    {
        int count = 0;
        foreach(var owned in ownedFurnitures.Values)
        { 
            GUI.Label(new Rect(0, 600 + count * 30, 180, 30), $"{owned.furnitureId} : {owned.count} ��");
            count++;
        }
    }

}
