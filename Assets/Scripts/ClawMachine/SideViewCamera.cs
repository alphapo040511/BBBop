using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideViewCamera : MonoBehaviour
{
    [Tooltip("카메라 위치")]public Transform[] viewPoint;

    public float viewChangeSpeed = 3f;
    public int currentIndex = 0;

    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
        currentIndex = 0;

    }

    private void Update()
    {
        cam.position = Vector3.Lerp(cam.position, viewPoint[currentIndex].position, Time.deltaTime * viewChangeSpeed);
        cam.rotation = Quaternion.Lerp(cam.rotation, viewPoint[currentIndex].rotation, Time.deltaTime * viewChangeSpeed);

        if (Input.GetKeyDown(KeyCode.G))
        {
            NextView();
        }
    }

    void NextView()
    {
        currentIndex++;
        if (currentIndex >= viewPoint.Length)
        {
            currentIndex = 0;
        }
    }
}
