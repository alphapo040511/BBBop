using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class CraneController : MonoBehaviour
{
    [Header("����")]
    public Transform originalClawPivot;         //ClawPivot

    [Header("ȸ�� ����")]
    public float upperStartAngle = 100f;
    public float upperEndAngle = 150f;
    public float lowerStartAngle = 40f;
    public float lowerEndAngle = 75f;

    [Header("���� ����")]
    [Tooltip("���� �ݴ� �ӵ�")] public float clawSpeed = 3f;
    [Tooltip("���� ���� ����")]public float radiusFromCenter = 0.8f;

    [Header("�ʱ� ��ġ ����")]
    public Vector3 initalPosition = new Vector3(0, 5, 0);           //ũ���� ���� ��ġ

    [Header("���� 3D �̵� ����")]
    public float moveSpeed = 4f;            //���� �̵� �ӵ�
    public float verticalSpeed = 3f;        //���� �̵� �ӵ�
    public float maxMoveRange = 6f;         //x/z �� �̵� ����
    public float maxHeight = 5f;            //�ִ� ����
    public float minHeight = 0.5f;          //�ּ� ����(�ٴ�)

    [Header("�ڵ� ���� ����")]
    public float descendSpeed = 2f;         //�������� �ӵ�
    public float ascendSpeed = 1.5f;        //�ö���� �ӵ�
    public float grabWaitTime = 0.5f;       //���� ��� �ð�

    [Header("�浹 ���� ����")]
    public LayerMask grabbableLayer = -1;   //���� �� �ִ� ������Ʈ ���̾�
    public float detectionRadius = 0.3f;    //�浹 ���� ����
    public Transform detectionPoint;        //���� ����Ʈ (���� �߽�)

    [Header("��Ʈ��")]
    public KeyCode grabKey = KeyCode.Space;         //�ڵ� ����
    public KeyCode moveForwardKey = KeyCode.W;      //������ z+
    public KeyCode moveBackKey = KeyCode.S;         //�ڷ� z-
    public KeyCode moveLeftKey = KeyCode.A;         //���� x-
    public KeyCode moveRightKey = KeyCode.D;        //������ x+
    public KeyCode openClawKey = KeyCode.X;         //���� ������

    //���� ���� ������
    private Transform[] clawPivots = new Transform[3];
    private Transform[] lowerPivots = new Transform[3];
    private float clawProgress = 0f;

    //ũ���� ��ġ
    private Vector3 cranePositon = Vector3.zero;

    public enum GrabState
    {
        Idle,                           //��� ���� - ���� ���� ����
        Descending,                     //�������� �� - ��ü ���� ���
        Grabbing,                       //���� �� - ���� �ݱ� �ִϸ��̼�
        Ascending                       //�ö󰡴� �� - ���� ��ġ�� ����
    }

    private GrabState currentState = GrabState.Idle;
    private float grabTimer = 0f;
    private float originalHeight = 0f;
    private GameObject grabbedObject = null;            //���� ���� ������Ʈ

    private void Start()
    {
        if (originalClawPivot == null)
        {
            Debug.LogError("���� ClawPivot�� �������ּ���.");
            return;
        }

        //�ʱ�ȭ
        CreateThreeClaws();
        SetClawProgress(clawProgress);

        cranePositon = initalPosition;
        transform.position = cranePositon;

        if (detectionPoint == null)
        {
            detectionPoint = transform;
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateAutoGrab();
        UpdateMovement();
    }

    void CreateThreeClaws()
    {
        for (int i = 0; i < 3; i++)
        {
            Transform clawCopy;

            if (i == 0)
            {
                clawCopy = originalClawPivot;
                clawCopy.name = "Claw1_Pivot";
            }
            else
            {
                clawCopy = Instantiate(originalClawPivot, transform);
                clawCopy.name = $"Claw{i + 1}_Pivot";
            }

            //120���� ȸ���Ͽ� 3�� ���� ��ġ
            float angle = i * 120f;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 position = new Vector3(
                Mathf.Sin(radians) * radiusFromCenter,
                0,
                Mathf.Cos(radians) * radiusFromCenter
            );

            clawCopy.localPosition = position;
            clawCopy.localRotation = Quaternion.Euler(upperStartAngle, angle, 0);

            clawPivots[i] = clawCopy;

            Transform lowerPivot = clawCopy.Find("ClawLower_Pivot");
            if (lowerPivot != null)
            {
                lowerPivots[i] = lowerPivot;
                lowerPivot.localRotation = Quaternion.Euler(lowerStartAngle, 0 ,0);
            }
        }
    }

    void SetClawProgress(float progress)            //0~1 ���� ��
    {
        for (int i = 0; i < 3; i++)
        {
            if (clawPivots[i] != null)
            {
                float upperAngle = Mathf.Lerp(upperStartAngle, upperEndAngle, progress);
                float yRotation = i * 120f;
                clawPivots[i].localRotation = Quaternion.Euler(upperAngle, yRotation, 0);
            }

            if (lowerPivots[i] != null)
            {
                float lowerAngle = Mathf.Lerp(lowerStartAngle, lowerEndAngle, progress);
                lowerPivots[i].localRotation = Quaternion.Euler(lowerAngle, 0, 0);
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(grabKey) && currentState == GrabState.Idle)
        {
            StartAutoGrab();
        }

        if (Input.GetKeyDown(openClawKey))
        {
            OpenClaw();
        }
    }

    void UpdateMovement()
    {
        if (currentState != GrabState.Idle) return;

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(moveForwardKey))
        {
            movement.z += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(moveBackKey))
        {
            movement.z -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(moveLeftKey))
        {
            movement.x -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(moveRightKey))
        {
            movement.x += moveSpeed * Time.deltaTime;
        }

        cranePositon += movement;
        cranePositon.x = Mathf.Clamp(cranePositon.x, -maxMoveRange, maxMoveRange);
        cranePositon.z = Mathf.Clamp(cranePositon.z, -maxMoveRange, maxMoveRange);
        cranePositon.y = initalPosition.y;      //�׻� �ʱ� ���� ����

        transform.position = cranePositon;
    }

    bool CheckForObstacle()
    {
        Vector3 checkPosition = detectionPoint.position + Vector3.down * 0.5f;
        Collider[] nearbyObjects = Physics.OverlapSphere(checkPosition, detectionRadius, grabbableLayer);

        if (nearbyObjects.Length > 0)
        {
            Debug.Log($"������ ��ü �� : {nearbyObjects.Length}, üũ ��ġ : {checkPosition}");
        }

        return nearbyObjects.Length > 0;
    }

    void TryGrabObject()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(detectionPoint.position, detectionRadius, grabbableLayer);

        if (nearbyObjects.Length > 0)
        {
            float closeseDistance = float.MaxValue;
            Collider closestObject = null;

            foreach (Collider obj in nearbyObjects)
            {
                float distance = Vector3.Distance(detectionPoint.position, obj.transform.position);

                if (distance < closeseDistance)
                {
                    closeseDistance = distance;
                    closestObject = obj;
                }
            }

            if (closestObject != null)
            {
                GrabObject(closestObject.gameObject);
            }
        }
    }

    void UpdateAutoGrab()
    {
        switch (currentState)
        {
            case GrabState.Descending:
                cranePositon.y -= descendSpeed * Time.deltaTime;
                transform.position = cranePositon;

                if (cranePositon.y <= (originalHeight - 0.5f) && CheckForObstacle())
                {
                    currentState = GrabState.Grabbing;
                    grabTimer = 0f;
                }
                else if (cranePositon.y <= minHeight)
                {
                    cranePositon.y = minHeight;
                    transform.position = cranePositon;
                    currentState = GrabState.Grabbing;
                    grabTimer = 0f;
                }
                break;
            case GrabState.Grabbing:
                grabTimer += Time.deltaTime;
                clawProgress = Mathf.Lerp(0f, 1f, grabTimer / grabWaitTime);
                SetClawProgress(clawProgress);

                if (grabTimer >= grabWaitTime)
                {
                    TryGrabObject();
                    currentState = GrabState.Ascending;
                }
                break;

            case GrabState.Ascending:
                cranePositon.y += ascendSpeed * Time.deltaTime;
                transform.position = cranePositon;

                if (cranePositon.y >= originalHeight)
                {
                    cranePositon.y = originalHeight;
                    transform.position = cranePositon;
                    currentState = GrabState.Idle;
                }
                break;
        }
    }

    void StartAutoGrab()
    {
        originalHeight = initalPosition.y;
        currentState = GrabState.Descending;
        clawProgress = 0f;
        SetClawProgress(clawProgress);
    }

    void OpenClaw()
    {
        clawProgress = 0f;
        SetClawProgress(clawProgress);
        currentState = GrabState.Idle;

        if (grabbedObject != null)
        {
            ReleaseObjcet();
        }
    }

    void GrabObject(GameObject obj)
    {
        grabbedObject = obj;
        grabbedObject.transform.SetParent(transform);

        Rigidbody rb = grabbedObject. GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void ReleaseObjcet()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.SetParent(null);

            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            Debug.Log("��ü�� ���ҽ��ϴ� : " + grabbedObject.name);
            grabbedObject = null;
        }
    }

    
}
