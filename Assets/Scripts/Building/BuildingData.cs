using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building Data", menuName = "Custom Asset/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("건물 설정")]
    public PlacedItem buildingPrefab;

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
    public int goldAmount;              // 골드 생산량
    public float intervalTime;          // 골드 생산 주기
}
