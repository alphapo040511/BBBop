using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

// 저장용 class
[System.Serializable]
public class OwnedFurniture
{
    public Action<int> onChangeCount;

    public string furnitureId;
    public int count;  // 보유 중인 개수

    public OwnedFurniture(string furnitureId, int count)
    {
        this.furnitureId = furnitureId;
        this.count = count;
    }
}

// 설치된 가구
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
    public List<FurnitureData> furnitureDatas = new List<FurnitureData>();       // 임시 가구 정보 리스트

    private Dictionary<string, OwnedFurniture> ownedFurnitures = new Dictionary<string, OwnedFurniture>();                          // 보유 가구
    private Dictionary<Vector2Int, PlacedFurnitureData> placedFurnitures = new Dictionary<Vector2Int, PlacedFurnitureData>();       // 설치된 가구


    // 외부 읽기 전용 Dictionary
    public ReadOnlyDictionary<string, OwnedFurniture> OwnedFurnitures => new ReadOnlyDictionary<string, OwnedFurniture>(ownedFurnitures);
    public ReadOnlyDictionary<Vector2Int, PlacedFurnitureData> PlacedFurnitures => new ReadOnlyDictionary<Vector2Int, PlacedFurnitureData>(placedFurnitures);


    // 아이템 획득
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
        Debug.Log($"(ID:{id}) 보유 개수 : {ownedFurnitures[id].count}");
    }

    // 가구 사용
    public bool UseFurniture(string id)
    {
        if (ownedFurnitures.ContainsKey(id))
        {
            ownedFurnitures[id].count--;
            ownedFurnitures[id].onChangeCount?.Invoke(ownedFurnitures[id].count);
            Debug.Log($"(ID:{id}) 보유 개수 : {ownedFurnitures[id].count}");

            if (ownedFurnitures[id].count <= 0)
            {
                ownedFurnitures.Remove(id);
            }

            return true;
        }
        else
        {
            Debug.LogWarning($"ID:{id} 가구를 보유하고 있지 않습니다.");
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

    // 가구 설치
    public bool PlaceFurniture(PlacedFurnitureData data)
    {
        if (placedFurnitures.ContainsKey(data.start))
        {
            Debug.LogWarning($"{data.start} 위치에 이미 배치된 가구가 있습니다.");
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
            GUI.Label(new Rect(0, 600 + count * 30, 180, 30), $"{owned.furnitureId} : {owned.count} 개");
            count++;
        }
    }

}
