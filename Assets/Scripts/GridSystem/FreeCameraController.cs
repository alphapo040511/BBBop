using System.Collections.Generic;
using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public List<Camera> cameras = new List<Camera>();

    public Camera editCamera;

    [Header("카메라 이동 설정")]
    public float moveSpeed = 10f;
    public float fastMoveSpeed = 20f;
    public float mouseSensitivity = 2f;
    public float scrollSpeed = 5f;

    [Header("카메라 제한")]
    public float minY = 1f;
    public float maxY = 50f;
    public float maxSize = 5f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private float orthographicSize = 5f;
    private bool isMouseLook = false;

    void Start()
    {
        GameEvents.OnGameStateChanged += ChangeGameState;

        // 마우스 커서 설정
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 초기 회전값 설정
        Vector3 rotation = transform.eulerAngles;
        rotationX = rotation.x;
        rotationY = rotation.y;
    }

    private void OnDestroy()
    {
        GameEvents.OnGameStateChanged -= ChangeGameState;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleMouseScroll();
    }

    void HandleMouseLook()
    {
        // 우클릭으로 시점 회전 모드 토글
        if (Input.GetMouseButtonDown(1))
        {
            isMouseLook = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isMouseLook = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 마우스 시점 회전
        if (isMouseLook)
        {
            rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;

            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }
    }

    void HandleMovement()
    {
        // 이동 속도 (Shift로 빠른 이동)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;

        // WASD 이동
        Vector3 moveDirection = Vector3.zero;

        //if (Input.GetKey(KeyCode.W))
        //    moveDirection += transform.forward;
        //if (Input.GetKey(KeyCode.S))
        //    moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDirection += transform.right;

        // 수직 이동 (Q, E)
        if (Input.GetKey(KeyCode.W))
            moveDirection += transform.up;
        if (Input.GetKey(KeyCode.S))
            moveDirection -= transform.up;

        // 이동 적용
        transform.position += moveDirection.normalized * currentSpeed * Time.deltaTime;

        // Y축 제한
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void HandleMouseScroll()
    {
        // 마우스 휠로 앞뒤 이동
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            orthographicSize -= scroll * scrollSpeed;

            orthographicSize = Mathf.Clamp(orthographicSize, 2f, maxSize);

            for (int i = 0; i < cameras.Count; i++) 
            {
                cameras[i].orthographicSize = orthographicSize;
            }
        }
    }

    public void ChangeGameState(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.EditMode:
                editCamera.gameObject.SetActive(true);
                break;
            default:
                editCamera.gameObject.SetActive(false);
                break;
        }
    }
}