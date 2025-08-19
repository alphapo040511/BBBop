using UnityEngine;

public class SimplePlacer : MonoBehaviour
{
    [Header("������")]
    //public GameObject groundTilePrefab;
    public PlacedItem currentItem;

    [Header("���� ���")]
    public PlaceMode currentMode = PlaceMode.Ground;

    [Header("�����̾� ����")]
    public int conveyorDirection = 0; // 0=��, 1=��, 2=��, 3=��

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
            Debug.LogError("GridManager�� ã�� �� �����ϴ�!");
        }
    }

    void Update()
    {
        HandleInput();
        HandleMouseClick();
    }

    void HandleInput()
    {
        // ��ġ ��� ����
        if (Input.GetKeyDown(KeyCode.Alpha1))
            currentMode = PlaceMode.Ground;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            currentMode = PlaceMode.ArrowConveyor;
        else if (Input.GetKeyDown(KeyCode.X))
            currentMode = PlaceMode.Remove;

        // �����̾� ���� ���� (RŰ)
        if (Input.GetKeyDown(KeyCode.R))
        {
            conveyorDirection = (conveyorDirection + 1) % 4;
            currentItem.Rotation = GetDirectionAngle(conveyorDirection);
            Debug.Log($"�����̾� ����: {GetDirectionName(conveyorDirection)}");
        }
    }

    string GetDirectionName(int direction)
    {
        string[] names = { "��", "��", "��", "��" };
        return names[direction % 4];
    }

    int GetDirectionAngle(int direction)
    {
        int[] angles = { 0, 90, 180, 270 };
        return angles[direction % 4];
    }

    void RotateConveyorUnderMouse()
    {
        Debug.Log("RŰ ���� - �����̾� ȸ�� �õ�");

        Vector3 mousePos = Input.mousePosition;
        Ray ray = playerCamera.ScreenPointToRay(mousePos);

        if (ray.direction.y < 0)
        {
            float distance = -ray.origin.y / ray.direction.y;
            Vector3 hitPoint = ray.origin + ray.direction * distance;

            Vector2Int gridPos = gridManager.WorldToGridPosition(hitPoint);
            Debug.Log($"���콺 ���� ��ġ: ({gridPos.x}, {gridPos.y})");

            if (gridManager.IsValidGridPosition(gridPos.x, gridPos.y))
            {
                // �ش� ��ġ�� �����̾� ã��
                Vector3 worldPos = gridManager.GridToWorldPosition(gridPos.x, gridPos.y);
                Collider[] colliders = Physics.OverlapSphere(worldPos, 0.5f);

                Debug.Log($"ã�� ������Ʈ ��: {colliders.Length}");

                foreach (Collider col in colliders)
                {
                    Debug.Log($"������Ʈ �̸�: {col.name}");

                    // Building���� �����ϴ� ������Ʈ �߿��� ConveyorRotator ã��
                    if (col.name.StartsWith("Building"))
                    {
                        //ConveyorRotator rotator = col.GetComponent<ConveyorRotator>();
                        //if (rotator != null)
                        //{
                        //    Debug.Log("�����̾� ȸ�� ����!");
                        //    rotator.RotateToNextDirection();
                        //    return;
                        //}
                    }
                }

                Debug.Log("�ش� ��ġ�� �����̾ �����ϴ�.");
            }
        }
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // ��Ŭ��
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = playerCamera.ScreenPointToRay(mousePos);

            // �ٴ� ������ ������
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

                    // �����̾ ���� ����
                    if (success)
                    {
                        Debug.Log($"�����̾� ��ġ: {GetDirectionName(conveyorDirection)} ����");
                    }
                }
                break;

            case PlaceMode.Remove:
                success = gridManager.RemoveBuilding(x, z);
                break;
        }

        if (success)
        {
            Debug.Log($"{currentMode} ��ġ: ({x}, {z})");
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 120), "��ġ ���");
        GUI.Label(new Rect(20, 30, 180, 20), "1: �ٴ�");
        GUI.Label(new Rect(20, 50, 180, 20), "2: ȭ��ǥ �����̾�");
        GUI.Label(new Rect(20, 70, 180, 20), "X: ����");
        GUI.Label(new Rect(20, 90, 180, 20), "R: �����̾� ���� ����");

        GUI.Label(new Rect(10, 140, 200, 20), $"���� ���: {currentMode}");

        if (currentMode == PlaceMode.ArrowConveyor)
        {
            GUI.Label(new Rect(10, 160, 200, 20), $"����: {GetDirectionName(conveyorDirection)}");
        }
    }
}