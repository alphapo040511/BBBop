using UnityEngine;
using System.Collections.Generic;


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

    public bool AreAllTilesValid(Vector2Int start, Vector2Int size, int rotation)
    {
        foreach(Vector2Int pos in GetCoveredTiles(start, size, rotation))
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
    public bool HasAnyBuildingInRange(Vector2Int start, Vector2Int size, int rotation)
    {
        foreach (Vector2Int pos in GetCoveredTiles(start, size, rotation))
        {
            if (buildingsGrid[pos.x, pos.y] != null) return true;
        }
        return false;
    }

    // 건물 배치
    public bool PlaceBuilding(Vector2Int start, BuildingData item, int rotation)
    {
        if (!AreAllTilesValid(start, item.size, rotation)) 
        {
            Debug.Log("범위 밖임");
            return false; 
        }

        // 해당 위치에 이미 건물이 있는지 확인 (있다면 return)
        if (HasAnyBuildingInRange(start, item.size, rotation))
        {
            Debug.Log("이미 건물 있음");
            return false;
        }


        PlacedItem newBuilding = Instantiate(item.buildingPrefab, new Vector3(start.x + 0.5f, 0, start.y + 0.5f), Quaternion.Euler(0, rotation, 0));
        newBuilding.transform.SetParent(buildingsLayer);
        newBuilding.Start = start;
        newBuilding.Rotation = rotation;

        // 건물 범위 채우기
        foreach (var pos in GetCoveredTiles(start, item.size, rotation))
        {
            buildingsGrid[pos.x, pos.y] = newBuilding;
        }


        return true;
    }

    // 건물 제거
    public bool RemoveBuilding(int x, int z)
    {
        if (!IsValidGridPosition(x, z)) return false;

        if (buildingsGrid[x, z] != null)
        {
            PlacedItem target = buildingsGrid[x, z];
            foreach (var item in GetCoveredTiles(target.Start, target.Size, target.Rotation))
            {
                Debug.Log($"{item.x},{item.y}칸 비움");
                buildingsGrid[item.x, item.y] = null;
            }
            Debug.Log(target.name + "칸의 아이템 삭제");
            Destroy(target.gameObject);
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
                foreach (Vector2Int pos in GetCoveredTiles(item.Start, item.Size, item.Rotation))
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

    public IEnumerable<Vector2Int> GetCoveredTiles(Vector2Int start, Vector2Int size, int rotation)
    {
        int w = size.x;
        int h = size.y;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector2Int offset = new Vector2Int(x, y);

                // Rotation 처리
                switch (rotation % 360)
                {
                    case 0: // 그대로
                        break;

                    case 90:
                        offset = new Vector2Int(y, -x);
                        break;

                    case 180:
                        offset = new Vector2Int(-x, -y);
                        break;

                    case 270:
                        offset = new Vector2Int(-y, x);
                        break;
                }

                yield return start + offset;
            }
        }
    }
}