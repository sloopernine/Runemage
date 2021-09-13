using System;
using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;

public class RuneCloud : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private SphereCollider trigger;

    public List<Vector3> pointCloudList = new List<Vector3>();

    private Result result;

    public float spellThreshold;
    public float fadeTime;
    private float fadeCounter;
    
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        trigger = GetComponent<SphereCollider>();
        
        lineRenderer.positionCount = 1;
        pointCloudList.Add(transform.position);
        lineRenderer.SetPosition(0, transform.position);

        fadeCounter = fadeTime;
    }

    private void Update()
    {
        if (fadeCounter > 0)
        {
            fadeCounter -= Time.deltaTime;
        }
        else
        {
            //TODO kill me
        }
    }

    public void AddPoint(Vector3 point)
    {
        pointCloudList.Add(point);

        fadeCounter = fadeTime;
    }

    public void EndDraw()
    {
        Point[] pointArray = new Point[pointCloudList.Count];
        
        for (int i = 0; i < pointArray.Length; i++) 
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(pointCloudList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        result = RuneMaker.Instance.Classify(pointArray);
        
        ValidateSpell();
    }

    private void ValidateSpell()
    {
        if (result.Score >= spellThreshold)
        {
            //TODO instantiate spell
        }
        else
        {
            //TODO do we want to give feedback on too low threshold?
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        throw new NotImplementedException();
    }

    private void OnTriggerExit(Collider other)
    {
        throw new NotImplementedException();
    }
}


