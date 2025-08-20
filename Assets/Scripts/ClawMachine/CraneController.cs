using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    //���� ������ �Ķ���͵�
    [Header("���� ����")]
    public Transform originalClawPivot;     //ClawPivot

    [Header("ȸ�� ����")]
    public float upperStartAngle = 100f;
    public float upperEndAngle = 150f;
    public float lowerStartAngle = 40f;
    public float lowerEndAngle = 75f;

    [Header("���� ����")]
    public float clawSpeed = 3f;
    public float radiusFromCenter = 0.8f;

    [Header("�ʱ� ��ġ ����")]
    public Vector3 initalPosition = new Vector3(0, 5, 0);       //ũ���� ���� ��ġ

    [Header("3D �̵� ����")]
    public float moveSpeed = 4f;            //���� �̵� �ӵ�
    public float verticalSpeed = 3f;        //���� �̵� �ӵ�
    public float maxMoveRange = 6f;         //x/z �� �̵� ����
    public float maxHeight = 5f;            //�ִ� ����
    public float minHeight = 0.5f;          //�ּ� ���� (�ٴ�)

    [Header("�ڵ� ���� ����")]
    public float descendSpeed = 2f;         //�������� �ӵ�
    public float ascendSpeed = 1.5f;        //�ö���� �ӵ�
    public float grabWaitTime = 0.5f;       //���� ��� �ð�

    [Header("�浹 ���� ����")]
    public LayerMask grabbableLayer = -1;   //���� �� �ִ� ������Ʈ ���̾�
    public float detectionRadius = 0.3f;    //�浹 ���� ����
    public Transform detectionPoint;        //���� ����Ʈ (���� �߽�)

    [Header("��Ʈ��")]
    public KeyCode grabKey = KeyCode.Space;             //�ڵ� ����
    public KeyCode moveForwardKey = KeyCode.W;          //������ (z+)
    public KeyCode moveBackKey = KeyCode.S;             //�ڷ� (z-)
    public KeyCode movwLeftKey = KeyCode.A;             //���� (x-)
    public KeyCode moveRightKey = KeyCode.D;            //������ (x+)
    public KeyCode openClawKey = KeyCode.X;             //���� ������

    //���� ���� ������
    private Transform[] clawPivots = new Transform[3];
    private Transform[] lowerPivots = new Transform[3];
    private float clawProgress = 0f;

    //ũ���� ��ġ
    private Vector3 cranePosition = Vector3.zero;

    //���� ����
    public enum GrabState
    {
        Idle,           //��� ���� - ���� ���� ����
        Descending,     //�������� �� - ��ü ���� ���
        Grabbing,       //���� �� - ���� �ݱ� �ִϸ��̼�
        Ascending       //�ö󰡴� �� - ���� ��ġ�� ����
    }

    private GrabState currentState = GrabState.Idle;
    private float grabTimer = 0f;
    private float originalHeight = 0f;
    private GameObject grabbedObject = null;            //���� ���� ������Ʈ

    private void Start()
    {
        if (originalClawPivot == null)
        {
            Debug.LogError("���� ClawPivot�� �������ּ���!");
            return;
        }

        //�ʱ�ȭ
        CreateThreeClaws();
        SetClawProgress(clawProgress);

        //�ʱ� ��ġ ���� (ũ������ �׻� �� ���̿��� �����ϰ� ���ƿ�)
        cranePosition = initalPosition;
        transform.position = cranePosition;

        //���� ����Ʈ�� �������� �ʾҴٸ� �ڵ����� �߽��� ���
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

            //�� �ѷ��� ��ġ�ϴ� ���� ����
            Vector3 position = new Vector3(
                Mathf.Sin(radians) * radiusFromCenter,
                0,
                Mathf.Cos(radians) * radiusFromCenter
            );

            clawCopy.localPosition = position;
            clawCopy.localRotation = Quaternion.Euler(upperStartAngle, angle, 0);

            clawPivots[i] = clawCopy;

            //���� ���� ã��
            Transform lowerPivot = clawCopy.Find("ClawLower_Pivot");
            if (lowerPivot != null)
            {
                lowerPivots[i] = lowerPivot;
                lowerPivot.localRotation = Quaternion.Euler(lowerStartAngle, 0, 0);
            }
        }
    }

    void SetClawProgress(float progress)                //0~1 ���� ��
    {
        for (int i = 0; i < 3; i++)
        {
            if (clawPivots[i] != null)
            {
                //���� �������� �ε巯�� �ִϸ��̼� ����
                float upperAngle = Mathf.Lerp(upperStartAngle, upperEndAngle, progress);
                float yRotation = i * 120;
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
        //�ڵ� ���� ���� (SpaceŰ)
        if (Input.GetKeyDown(grabKey) && currentState == GrabState.Idle)
        {
            Debug.Log("Space Ű ����! �ڵ� ���� ���� �õ�");
            StartAutoGrab();
        }

        //���� ���� ���� (xŰ)
        if (Input.GetKeyDown(openClawKey))
        {
            Debug.Log("XŰ ����! ���� ������");
            OpenClaw();
        }

        //������ - ���� ���� ���
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"���� ���� : {currentState}");
            Debug.Log($"���� ��ġ : {cranePosition}");
            Debug.Log($"Transform ��ġ : {transform.position}");
        }
    }

    void UpdateMovement()
    {
        //�ڵ� ���� ���� �ƴ� ���� ���� ���� ����
        if (currentState != GrabState.Idle) return;

        Vector3 movement = Vector3.zero;

        //3D �̵� (x, z�ุ - ũ���� ��Ʈ��ó��
        if (Input.GetKey(moveForwardKey))
        {
            movement.z += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(moveBackKey))
        {
            movement.z -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(movwLeftKey))
        {
            movement.x -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(moveRightKey))
        {
            movement.x += moveSpeed * Time.deltaTime;
        }

        //��ġ ������Ʈ �� ���� ���� (Y���� ���� - initialPosition.y ����)
        cranePosition += movement;
        cranePosition.x = Mathf.Clamp(cranePosition.x, -maxMoveRange, maxMoveRange);
        cranePosition.z = Mathf.Clamp(cranePosition.z, -maxMoveRange, maxMoveRange);
        cranePosition.y = initalPosition.y;     //�׻� �ʱ� ���� ����

        //���� ��ġ ����
        transform.position = cranePosition;
    }

    bool CheckForObstacle()
    {
        //���� �ٷ� �Ʒ��ʿ� ��ü�� ������ �ִ��� ���� (�� �����ϰ�)
        Vector3 checkPosition = detectionPoint.position + Vector3.down * 0.5f;      //0.5��ŭ �Ʒ����� üũ
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

            //�Ÿ� �񱳷� ���� ����� ��ü ã��
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
                //�������� (���� �Ÿ� ������ �Ŀ��� �浹 ����)
                cranePosition.y -= descendSpeed * Time.deltaTime;
                transform.position = cranePosition;

                //0.5�� �̻� ������ �Ŀ��� ��ü ���� ����
                if (cranePosition.y <= (originalHeight - 0.5f) && CheckForObstacle())
                {
                    currentState = GrabState.Grabbing;
                    grabTimer = 0f;
                    Debug.Log("��ü ����! ���� ����");
                }
                else if (cranePosition.y <= minHeight)
                {
                    cranePosition.y = minHeight;
                    transform.position = cranePosition;
                    currentState = GrabState.Grabbing;
                    grabTimer = 0f;
                    Debug.Log("�ٴڿ� ����! ���� ����");
                }
                break;

            case GrabState.Grabbing:
                //���� (��� ����ϸ鼭 ���� ���Ǹ���)
                grabTimer += Time.deltaTime;
                clawProgress = Mathf.Lerp(0f, 1f, grabTimer / grabWaitTime);
                SetClawProgress(clawProgress);

                if (grabTimer >= grabWaitTime)
                {
                    TryGrabObject();
                    currentState = GrabState.Ascending;
                    Debug.Log("���� �Ϸ�! �ö󰡱� ����");
                }
                break;

            case GrabState.Ascending:
                //�ö󰡱�
                cranePosition.y += ascendSpeed * Time.deltaTime;
                transform.position = cranePosition;

                Debug.Log($"�ö󰡴� �� : ���� {cranePosition.y:F1}, ��ǥ {originalHeight:F1}");

                if (cranePosition.y >= originalHeight)
                {
                    cranePosition.y = originalHeight;
                    transform.position = cranePosition;
                    currentState = GrabState.Idle;
                    Debug.Log("���� ���� ����! �Ϸ�");
                }
                break;
        }
    }

    void StartAutoGrab()
    {
        originalHeight = initalPosition.y;          //��� �ʱ� ���̷� ���ư�
        currentState = GrabState.Descending;
        clawProgress = 0f;
        SetClawProgress(clawProgress);
        Debug.Log($"�ڵ� ���� ����! ���� ����: {originalHeight}");
    }

    void OpenClaw()
    {
        clawProgress = 0f;
        SetClawProgress(clawProgress);
        currentState = GrabState.Idle;

        if (grabbedObject != null)
        {
            ReleaseObject();
        }
    }

    void GrabObject(GameObject obj)
    {
        grabbedObject = obj;
        grabbedObject.transform.SetParent(transform);           //ũ���� �ڽ����� ����

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;      //���� �ùķ��̼� ��Ȱ��ȭ
        }

        Debug.Log("��ü�� ��ҽ��ϴ� : " + grabbedObject.name);
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.SetParent(null);        //�θ� ���� ����

            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;     //���� �ùķ��̼� ��Ȱ��ȭ
            }

            Debug.Log("��ü�� ���ҽ��ϴ� : " + grabbedObject.name);
            grabbedObject = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            //���� ���� �ð�ȭ
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionPoint.position, detectionRadius);

            //���� üũ ��ġ �ð�ȭ
            Gizmos.color = Color.red;
            Vector3 checkPosition = detectionPoint.position + Vector3.down * 0.5f;
            Gizmos.DrawWireSphere(checkPosition, detectionRadius);
        }
    }
}
