using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneReward : MonoBehaviour
{
    [SerializeField] FurnitureData furnitureData;
    public Collider objectCheckCollider;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Grabbable"))
        {
            Debug.Log("»Ì±â ¼º°ø");
        }
    }
}
