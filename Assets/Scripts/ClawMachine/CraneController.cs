using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class CraneController : MonoBehaviour
{
    [Header("집게")]
    public Transform originalClawPivot;         //ClawPivot

    [Header("회전 범위")]
    public float upperStartAngle = 100f;
    public float upperEndAngle = 150f;
    public float lowerStartAngle = 40f;
    public float lowerEndAngle = 75f;

    [Header("집게 설정")]
    [Tooltip("집게 닫는 속도")] public float clawSpeed = 3f;
    [Tooltip("집게 사이 간격")]public float radiusFromCenter = 0.8f;

    [Header("초기 위치 설정")]
    public Vector3 initalPosition = new Vector3(0, 5, 0);           //크레인 시작 위치

    [Header("집게 3D 이동 설정")]
    public float moveSpeed = 4f;            //수평 이동 속도
    public float verticalSpeed = 3f;        //상하 이동 속도
    public float maxMoveRange = 6f;         //x/z 축 이동 범위
    public float maxHeight = 5f;            //최대 높이
    public float minHeight = 0.5f;          //최소 높이(바닥)

    [Header("자동 집기 설정")]
    public float descendSpeed = 2f;         //내려가는 속도
    public float ascendSpeed = 1.5f;        //올라오는 속도
    public float grabWaitTime = 0.5f;       //집는 대기 시간

    [Header("충돌 감지 설정")]
    public LayerMask grabbableLayer = -1;   //잡을 수 있는 오브젝트 레이어
    public float detectionRadius = 0.3f;    //충돌 감지 범위
    public Transform detectionPoint;        //감지 포인트 (집게 중심)

    [Header("컨트롤")]
    public KeyCode grabKey = KeyCode.Space;         //자동 집기
    public KeyCode moveForwardKey = KeyCode.W;      //앞으로 z+
    public KeyCode moveBackKey = KeyCode.S;         //뒤로 z-
    public KeyCode moveLeftKey = KeyCode.A;         //왼쪽 x-
    public KeyCode moveRightKey = KeyCode.D;        //오른쪽 x+
    public KeyCode openClawKey = KeyCode.X;         //집게 벌리기

    //내부 상태 변수들
    private Transform[] clawPivots = new Transform[3];
    private Transform[] lowerPivots = new Transform[3];
    private float clawProgress = 0f;

    //크레인 위치
    private Vector3 cranePositon = Vector3.zero;

    public enum GrabState
    {
        Idle,                           //대기 상태 - 수동 조작 가능
        Descending,                     //내려가는 중 - 물체 감지 대기
        Grabbing,                       //집는 중 - 집게 닫기 애니메이션
        Ascending                       //올라가는 중 - 원래 위치로 복귀
    }

    private GrabState currentState = GrabState.Idle;
    private float grabTimer = 0f;
    private float originalHeight = 0f;
    private GameObject grabbedObject = null;            //현재 잡은 오브젝트

    private void Start()
    {
        if (originalClawPivot == null)
        {
            Debug.LogError("원본 ClawPivot을 설정해주세요.");
            return;
        }

        //초기화
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

            //120도씩 회전하여 3개 집게 배치
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

    void SetClawProgress(float progress)            //0~1 사이 값
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
        cranePositon.y = initalPosition.y;      //항상 초기 높이 유지

        transform.position = cranePositon;
    }

    bool CheckForObstacle()
    {
        Vector3 checkPosition = detectionPoint.position + Vector3.down * 0.5f;
        Collider[] nearbyObjects = Physics.OverlapSphere(checkPosition, detectionRadius, grabbableLayer);

        if (nearbyObjects.Length > 0)
        {
            Debug.Log($"감지된 물체 수 : {nearbyObjects.Length}, 체크 위치 : {checkPosition}");
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

            Debug.Log("물체를 놓았습니다 : " + grabbedObject.name);
            grabbedObject = null;
        }
    }

    
}
