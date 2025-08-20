using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideViewCamera : MonoBehaviour
{
    [Tooltip("ī�޶� ��ġ")]public Transform[] viewPoint;

    public float viewChangeSpeed = 3f;
    public int currentIndex = 0;

    private Transform cam;
    public Button but;

    private void Start()
    {
        cam = Camera.main.transform;
        currentIndex = 0;
        but.onClick.AddListener(NextView);
    }

    private void Update()
    {
        cam.position = Vector3.Lerp(cam.position, viewPoint[currentIndex].position, Time.deltaTime * viewChangeSpeed);
        cam.rotation = Quaternion.Lerp(cam.rotation, viewPoint[currentIndex].rotation, Time.deltaTime * viewChangeSpeed);

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
