using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    [Header("ī�޶� �̵� ����")]
    public float moveSpeed = 10f;
    public float fastMoveSpeed = 20f;
    public float mouseSensitivity = 2f;
    public float scrollSpeed = 5f;

    [Header("ī�޶� ����")]
    public float minY = 1f;
    public float maxY = 50f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isMouseLook = false;

    void Start()
    {
        // ���콺 Ŀ�� ����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // �ʱ� ȸ���� ����
        Vector3 rotation = transform.eulerAngles;
        rotationX = rotation.x;
        rotationY = rotation.y;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleMouseScroll();
    }

    void HandleMouseLook()
    {
        // ��Ŭ������ ���� ȸ�� ��� ���
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

        // ���콺 ���� ȸ��
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
        // �̵� �ӵ� (Shift�� ���� �̵�)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;

        // WASD �̵�
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDirection += transform.right;

        // ���� �̵� (Q, E)
        if (Input.GetKey(KeyCode.Q))
            moveDirection -= transform.up;
        if (Input.GetKey(KeyCode.E))
            moveDirection += transform.up;

        // �̵� ����
        transform.position += moveDirection.normalized * currentSpeed * Time.deltaTime;

        // Y�� ����
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void HandleMouseScroll()
    {
        // ���콺 �ٷ� �յ� �̵�
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            transform.position += transform.forward * scroll * scrollSpeed;
        }
    }
}