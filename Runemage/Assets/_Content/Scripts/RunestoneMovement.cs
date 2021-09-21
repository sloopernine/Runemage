using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunestoneMovement : MonoBehaviour
{
    private Vector3 startPosition;
    [SerializeField] Vector3 movePosition;


    private void Start()
    {
        startPosition = transform.position;
    }

    public void MoveToPoint(Vector3 point)
    {
        Vector3 direction = point - transform.position;
    }

}
