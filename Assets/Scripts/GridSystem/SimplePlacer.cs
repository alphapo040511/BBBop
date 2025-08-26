using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SimplePlacer : MonoBehaviour
{

    public event System.Action<string> OnChangeFurniture;
    public Vector2Int targetPosition { get; private set; }

    [Header("UI ���")]
    public Button EnterButton;
    public Button ApplyButton;
    public Button CancleButton;
    public Button RemoveButton;

    [Header("���� ���")]
    public FurnitureData currentFurniture;

    [Header("���� ���")]
    public PlaceMode currentMode = PlaceMode.Ground;

    [Header("�����̾� ����")]
    public int currentRotation = 0; // 0=��, 1=��, 2=��, 3=��

    private GridManager gridManager;
    private EditModeUI editModeUI;
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
        editModeUI = FindObjectOfType<EditModeUI>();
        playerCamera = Camera.main;

        if (gridManager == null)
        {
            Debug.LogError("GridManager�� ã�� �� �����ϴ�!");
        }

        if(EnterButton != null)
        {
            EnterButton.onClick.AddListener(EnterEditMode);
        }

        if (ApplyButton != null)
        {
            ApplyButton.onClick.AddListener(ApplyEdit);
        }

        if (CancleButton != null)
        {
            CancleButton.onClick.AddListener(CancleEdit);
        }

        if (RemoveButton != null)
        {
            RemoveButton.onClick.AddListener(RemoveMode);
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (EnterButton != null)
        {
            EnterButton.onClick.RemoveListener(EnterEditMode);
        }

        if (ApplyButton != null)
        {
            ApplyButton.onClick.RemoveListener(ApplyEdit);
        }

        if (CancleButton != null)
        {
            CancleButton.onClick.RemoveListener(CancleEdit);
        }

        if (RemoveButton != null)
        {
            RemoveButton.onClick.RemoveListener(RemoveMode);
        }
    }

    void Update()
    {
        //if (GameManager.Instance.currentGameState != GameState.EditMode) return;

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
        if (UnityEngine.EventSystems.EventSystem.current != null
            && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

        Vector3 mousePos = Input.mousePosition;
        Ray ray = playerCamera.ScreenPointToRay(mousePos);

        // �ٴ� ������ ������
        if (ray.direction.y < 0)
        {
            float distance = -ray.origin.y / ray.direction.y;
            Vector3 hitPoint = ray.origin + ray.direction * distance;

            targetPosition = gridManager.WorldToGridPosition(hitPoint);
        }

        if (Input.GetMouseButtonDown(0)) // ��Ŭ��
        {
            // UI �� Ŭ���̸� ����
            if (UnityEngine.EventSystems.EventSystem.current != null
                && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

                if (gridManager.IsValidGridPosition(targetPosition.x, targetPosition.y))
                { 
                    PlaceObject(targetPosition.x, targetPosition.y);
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
                if (currentFurniture != null)
                {
                    success = gridManager.PlaceBuilding(new Vector2Int(x, z), currentFurniture, GetDirectionAngle(currentRotation));
                }
                break;

            case PlaceMode.Remove:
                success = gridManager.RemoveFurniture(x, z);
                break;
        }

        if (success)
        {
            Debug.Log($"{currentMode} ��ġ: ({x}, {z})");
        }
    }

    public void EnterEditMode()
    {
        EnterButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
        GameManager.Instance.ChangeGameState(GameState.EditMode);
    }

    public void ApplyEdit()
    {
        SaveManager.Instance.SaveData();
        ExitEditMode();
    }

    public void CancleEdit()
    {
        gridManager.LoadFurnitureData();
        editModeUI.ResetUI();
        ExitEditMode();
    }

    public void RemoveMode()
    {
        currentMode = PlaceMode.Remove;
    }    

    public void ExitEditMode()
    {
        EnterButton.gameObject.SetActive(true);
        gameObject.SetActive(false);
        GameManager.Instance.ChangeGameState(GameState.Playing);
    }

    public void SelectFurnitureChange(FurnitureData furnitureData)
    {
        currentFurniture = furnitureData;
        OnChangeFurniture?.Invoke(furnitureData.id);
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 120), "��ġ ���");
        GUI.Label(new Rect(20, 30, 180, 20), "1: �ٴ�");
        GUI.Label(new Rect(20, 50, 180, 20), "2: ����");
        GUI.Label(new Rect(20, 70, 180, 20), "X: ����");
        GUI.Label(new Rect(20, 90, 180, 20), "R: ���� ���� ����");

        GUI.Label(new Rect(10, 140, 200, 20), $"���� ���: {currentMode}");

        if (currentMode == PlaceMode.Edit)
        {
            GUI.Label(new Rect(10, 160, 200, 20), $"����: {GetDirectionName(currentRotation)}");
        }
    }
}