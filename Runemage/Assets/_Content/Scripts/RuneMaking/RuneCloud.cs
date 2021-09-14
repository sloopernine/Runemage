using System;
using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using Singletons;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;
using Valve.VR;

public class RuneCloud : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private SphereCollider trigger;

	public float newPositionThresholdDistance;

	public List<Vector3> pointCloudList = new List<Vector3>();

    private Result result;

    public float triggerStartSize;
    public float triggerSizeModifier;
    
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
        triggerStartSize = trigger.radius / 2;
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
        Vector3 lastPoint = pointCloudList[pointCloudList.Count - 1];

        float cloudSize = GetCloudSize();
        
        trigger.radius = cloudSize * triggerSizeModifier;
        
        if (Vector3.Distance(point, lastPoint) > newPositionThresholdDistance)
        {
            pointCloudList.Add(point);
            lineRenderer.positionCount = pointCloudList.Count;
            lineRenderer.SetPosition(pointCloudList.Count - 1, point);
        }
        else
        {
            lineRenderer.positionCount = pointCloudList.Count;
            lineRenderer.SetPosition(pointCloudList.Count - 1, point);
        }

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

    private float GetCloudSize()
    {
        if (pointCloudList.Count <= 2)
        {
            return triggerStartSize;
        }
        
        float distance = 0;
        Vector3 centroid = Vector3.zero;

        foreach (Vector3 point in pointCloudList)
        {
            float newDistance = Vector3.Distance(transform.position, point);
            centroid += point;
            
            if (distance < newDistance)
            {
                distance = newDistance;
            }
        }

        centroid = centroid / pointCloudList.Count;
        trigger.center = transform.InverseTransformPoint(centroid);
        
        return distance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RuneHand"))
        {
            other.GetComponent<RuneHand>().SetInRuneCloud(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RuneHand"))
        {
            other.GetComponent<RuneHand>().SetOutsideRuneCloud();
        }
    }
}