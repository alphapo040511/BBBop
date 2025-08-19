using UnityEngine;

public class SimplePlacer : MonoBehaviour
{
    [Header("프리팹")]
    //public GameObject groundTilePrefab;
    public PlacedItem currentItem;

    [Header("현재 모드")]
    public PlaceMode currentMode = PlaceMode.Ground;

    [Header("컨베이어 방향")]
    public int conveyorDirection = 0; // 0=북, 1=동, 2=남, 3=서

    private GridManager gridManager;
    private Camera playerCamera;

    public enum PlaceMode
    {
        Ground,
        ArrowConveyor,
        Remove
    }

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        playerCamera = Camera.main;

        if (gridManager == null)
        {
            Debug.LogError("GridManager를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        HandleInput();
        HandleMouseClick();
    }

    void HandleInput()
    {
        // 배치 모드 변경
        if (Input.GetKeyDown(KeyCode.Alpha1))
            currentMode = PlaceMode.Ground;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            currentMode = PlaceMode.ArrowConveyor;
        else if (Input.GetKeyDown(KeyCode.X))
            currentMode = PlaceMode.Remove;

        // 컨베이어 방향 변경 (R키)
        if (Input.GetKeyDown(KeyCode.R))
        {
            conveyorDirection = (conveyorDirection + 1) % 4;
            currentItem.Rotation = GetDirectionAngle(conveyorDirection);
            Debug.Log($"컨베이어 방향: {GetDirectionName(conveyorDirection)}");
        }
    }

    string GetDirectionName(int direction)
    {
        string[] names = { "북", "동", "남", "서" };
        return names[direction % 4];
    }

    int GetDirectionAngle(int direction)
    {
        int[] angles = { 0, 90, 180, 270 };
        return angles[direction % 4];
    }

    void RotateConveyorUnderMouse()
    {
        Debug.Log("R키 눌림 - 컨베이어 회전 시도");

        Vector3 mousePos = Input.mousePosition;
        Ray ray = playerCamera.ScreenPointToRay(mousePos);

        if (ray.direction.y < 0)
        {
            float distance = -ray.origin.y / ray.direction.y;
            Vector3 hitPoint = ray.origin + ray.direction * distance;

            Vector2Int gridPos = gridManager.WorldToGridPosition(hitPoint);
            Debug.Log($"마우스 격자 위치: ({gridPos.x}, {gridPos.y})");

            if (gridManager.IsValidGridPosition(gridPos.x, gridPos.y))
            {
                // 해당 위치의 컨베이어 찾기
                Vector3 worldPos = gridManager.GridToWorldPosition(gridPos.x, gridPos.y);
                Collider[] colliders = Physics.OverlapSphere(worldPos, 0.5f);

                Debug.Log($"찾은 오브젝트 수: {colliders.Length}");

                foreach (Collider col in colliders)
                {
                    Debug.Log($"오브젝트 이름: {col.name}");

                    // Building으로 시작하는 오브젝트 중에서 ConveyorRotator 찾기
                    if (col.name.StartsWith("Building"))
                    {
                        //ConveyorRotator rotator = col.GetComponent<ConveyorRotator>();
                        //if (rotator != null)
                        //{
                        //    Debug.Log("컨베이어 회전 실행!");
                        //    rotator.RotateToNextDirection();
                        //    return;
                        //}
                    }
                }

                Debug.Log("해당 위치에 컨베이어가 없습니다.");
            }
        }
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = playerCamera.ScreenPointToRay(mousePos);

            // 바닥 평면과의 교차점
            if (ray.direction.y < 0)
            {
                float distance = -ray.origin.y / ray.direction.y;
                Vector3 hitPoint = ray.origin + ray.direction * distance;

                Vector2Int gridPos = gridManager.WorldToGridPosition(hitPoint);

                currentItem.Start = gridPos;

                if (gridManager.AreAllTilesValid(currentItem))
                {
                    PlaceObject(gridPos.x, gridPos.y);
                }
            }
        }
    }

    void PlaceObject(int x, int z)
    {
        bool success = false;

        switch (currentMode)
        {
            //case PlaceMode.Ground:
            //    if (groundTilePrefab != null)
            //        success = gridManager.PlaceGroundTile(groundTilePrefab, x, z);
            //    break;

            case PlaceMode.ArrowConveyor:
                if (currentItem != null)
                {
                    success = gridManager.PlaceBuilding(currentItem, x, z);

                    // 컨베이어에 방향 설정
                    if (success)
                    {
                        Debug.Log($"컨베이어 배치: {GetDirectionName(conveyorDirection)} 방향");
                    }
                }
                break;

            case PlaceMode.Remove:
                success = gridManager.RemoveBuilding(x, z);
                break;
        }

        if (success)
        {
            Debug.Log($"{currentMode} 배치: ({x}, {z})");
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 120), "배치 모드");
        GUI.Label(new Rect(20, 30, 180, 20), "1: 바닥");
        GUI.Label(new Rect(20, 50, 180, 20), "2: 화살표 컨베이어");
        GUI.Label(new Rect(20, 70, 180, 20), "X: 제거");
        GUI.Label(new Rect(20, 90, 180, 20), "R: 컨베이어 방향 변경");

        GUI.Label(new Rect(10, 140, 200, 20), $"현재 모드: {currentMode}");

        if (currentMode == PlaceMode.ArrowConveyor)
        {
            GUI.Label(new Rect(10, 160, 200, 20), $"방향: {GetDirectionName(conveyorDirection)}");
        }
    }
}