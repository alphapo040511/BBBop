using UnityEngine;

public class SimplePlacer : MonoBehaviour
{
    [Header("���� ���")]
    public BuildingData currentBuilding;

    [Header("���� ���")]
    public PlaceMode currentMode = PlaceMode.Ground;

    [Header("�����̾� ����")]
    public int currentRotation = 0; // 0=��, 1=��, 2=��, 3=��

    private GridManager gridManager;
    private Camera playerCamera;

    public enum PlaceMode
    {
        Ground,
        Edit,
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
            currentMode = PlaceMode.Edit;
        else if (Input.GetKeyDown(KeyCode.X))
            currentMode = PlaceMode.Remove;

        // �����̾� ���� ���� (RŰ)
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotation = (currentRotation + 1) % 4;
            Debug.Log($"�����̾� ����: {GetDirectionName(currentRotation)}");
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
                // �ش� ��ġ�� �ش��ϴ� ����� ��ġ �������� ���� �����ϸ� ȸ��
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

                PlaceObject(gridPos.x, gridPos.y);
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

            case PlaceMode.Edit:
                if (currentBuilding != null)
                {
                    success = gridManager.PlaceBuilding(new Vector2Int(x, z), currentBuilding, GetDirectionAngle(currentRotation));

                    // �����̾ ���� ����
                    if (success)
                    {
                        Debug.Log($"�����̾� ��ġ: {GetDirectionName(currentRotation)} ����");
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

        if (currentMode == PlaceMode.Edit)
        {
            GUI.Label(new Rect(10, 160, 200, 20), $"����: {GetDirectionName(currentRotation)}");
        }
    }
}