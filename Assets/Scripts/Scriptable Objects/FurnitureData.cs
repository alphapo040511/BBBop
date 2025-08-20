using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Furniture Data", menuName = "Custom Asset/Furniture Data")]
public class FurnitureData : ScriptableObject
{
    [Header("기본 설정")]
    public string id;
    public PlacedFurniture buildingPrefab;
    public Sprite icon;

    public Vector2Int size
    {
        get
        {
            if (buildingPrefab != null)
            {
                return new Vector2Int(buildingPrefab.width, buildingPrefab.depth);
            }
            else
            {
                Debug.LogWarning("buildingPrefab 이 할당되지 않았습니다.");
                return new Vector2Int(0, 0);
            }
        }
    }

    [Header("자원 생산 설정")]
    public bool canProduceResource;     // 자원 생산 가능 여부
    public int goldAmount = 1;              // 골드 생산량
    public float intervalTime = 1;          // 골드 생산 주기
}
