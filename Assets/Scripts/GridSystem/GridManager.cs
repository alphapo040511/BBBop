using UnityEngine;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;

public class GridManager : MonoBehaviour
{
    [Header("격자 설정")]
    public int gridWidth = 16;
    public int gridHeight = 16;
    public float cellSize = 1f;

    [Header("레이어 관리")]
    public Transform buildingsLayer;

    [Header("시각화")]
    public bool showGridLines = true;
    public Color gridColor = Color.white;

    // 격자 데이터 저장
    private PlacedItem[,] buildingsGrid;

    void Start()
    {
        InitializeGrid();
        CreateLayers();
    }

    void InitializeGrid()
    {
        buildingsGrid = new PlacedItem[gridWidth, gridHeight];
    }

    void CreateLayers()
    {
        if (buildingsLayer == null)
        {
            GameObject buildings = new GameObject("BuildingsLayer");
            buildings.transform.SetParent(transform);
            buildingsLayer = buildings.transform;
        }
    }

    // 격자 좌표를 월드 좌표로 변환 (격자 중앙에 배치)
    public Vector3 GridToWorldPosition(int x, int z)
    {
        return new Vector3(x * cellSize + cellSize * 0.5f, 0, z * cellSize + cellSize * 0.5f);
    }

    // 월드 좌표를 격자 좌표로 변환
    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int z = Mathf.FloorToInt(worldPos.z / cellSize);
        return new Vector2Int(x, z);
    }

    public bool AreAllTilesValid(PlacedItem item)
    {
        foreach(Vector2Int pos in item.GetCoveredTiles())
        {
            if (!IsValidGridPosition(pos.x, pos.y))
            {
                Debug.Log($"{pos.x},{pos.y}가 범위 밖임");
                return false;    // 하나라도 범위 밖이라면 false
            }
        }

        return true;
    }

    // 격자 좌표가 유효한지 확인
    public bool IsValidGridPosition(int x, int z)
    {
        return x >= 0 && x < gridWidth && z >= 0 && z < gridHeight;
    }

    // 범위 안에 건물이 있는지 확인 (있다면 true)
    public bool HasAnyBuildingInRange(PlacedItem item)
    {
        foreach (Vector2Int pos in item.GetCoveredTiles())
        {
            if (buildingsGrid[pos.x, pos.y] != null) return true;
        }
        return false;
    }

    // 건물 배치
    public bool PlaceBuilding(PlacedItem item, int x, int z)
    {
        PlacedItem itemCopy = new PlacedItem
        {
            Start = new Vector2Int(x, z),
            Size = item.Size,
            Rotation = item.Rotation,
            Item = item.Item                            // 여기엔 실제 설치 오브젝트는 나중에 할당
        };

        if (!AreAllTilesValid(itemCopy)) 
        {
            Debug.Log("범위 밖임");
            return false; 
        }

        // 해당 위치에 이미 건물이 있는지 확인 (있다면 return)
        if (HasAnyBuildingInRange(itemCopy))
        {
            Debug.Log("이미 건물 있음");
            return false;
        }

            // 새 건물 생성
        Vector3 worldPos = GridToWorldPosition(itemCopy.Start.x, itemCopy.Start.y);
        GameObject newBuilding = Instantiate(itemCopy.Item, worldPos, Quaternion.Euler(0, itemCopy.Rotation, 0));
        itemCopy.Item = newBuilding;                                                                                // 실제 설치 오브젝트 할당
        newBuilding.transform.SetParent(buildingsLayer);
        newBuilding.name = $"Building_{itemCopy.Start.x}_{itemCopy.Start.y}";

        // 건물 범위 채우기
        foreach (var pos in itemCopy.GetCoveredTiles())
        {
            buildingsGrid[pos.x, pos.y] = itemCopy;
        }


        return true;
    }

    // 건물 제거
    public bool RemoveBuilding(int x, int z)
    {
        if (!IsValidGridPosition(x, z)) return false;

        if (buildingsGrid[x, z] != null)
        {
            GameObject deleteTarget = buildingsGrid[x, z].Item;
            foreach (var item in buildingsGrid[x, z].GetCoveredTiles())
            {
                buildingsGrid[item.x, item.y] = null;
                Debug.Log($"{item.x},{item.y}칸 비움");
            }
            Debug.Log(deleteTarget.name + "칸의 아이템 삭제");
            DestroyImmediate(deleteTarget);
            return true;
        }
        return false;
    }

    public bool GetBuildingAtPosition(int x, int z, out PlacedItem item)
    {
        item = null;       //기본값으로 null 반환
        if (!IsValidGridPosition(x, z) || buildingsGrid[x, z] == null)      //범위 밖이거나 해당 위치에 건물이 없는 경우 false
        {
            return false;
        }

        item = buildingsGrid[x, z];                //건물이 있는경우 해당 오브젝트 반환
        return true;
    }

    // 격자 시각화 (Scene 뷰에서) - 오브젝트 중심에 맞춤
    void OnDrawGizmos()
    {
        if (!showGridLines) return;

        Gizmos.color = gridColor;

        // 세로 선 (격자 중앙 기준)
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = new Vector3(x * cellSize, 0, 0);
            Vector3 end = new Vector3(x * cellSize, 0, gridHeight * cellSize);
            Gizmos.DrawLine(start, end);
        }

        // 가로 선 (격자 중앙 기준)
        for (int z = 0; z <= gridHeight; z++)
        {
            Vector3 start = new Vector3(0, 0, z * cellSize);
            Vector3 end = new Vector3(gridWidth * cellSize, 0, z * cellSize);
            Gizmos.DrawLine(start, end);
        }


        // 설치된 가구 표시
        if (buildingsGrid == null) return;

        Gizmos.color = Color.green * 0.5f; // 반투명 초록

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                PlacedItem item = buildingsGrid[x, z];
                if (item == null) continue;

                // 겹치는 타일이 많으므로, 각 타일별로 표시
                foreach (Vector2Int pos in item.GetCoveredTiles())
                {
                    Vector3 tileCenter = new Vector3(
                        pos.x * cellSize + cellSize * 0.5f,
                        0,
                        pos.y * cellSize + cellSize * 0.5f
                    );

                    Gizmos.DrawCube(tileCenter, new Vector3(cellSize, 0.1f, cellSize));
                }
            }
        }
    }
}