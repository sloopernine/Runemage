using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Content.Scripts.Data.Containers
{
    public class SubCloud : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        
        private List<Vector3> subCloud = new List<Vector3>();

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void AddPoints(List<Vector3> newPoints)
        {
            int index = 0;
            
            foreach (var point in newPoints)
            {
                subCloud.Add(point);
                
                lineRenderer.positionCount = newPoints.Count;
                lineRenderer.SetPosition(index, point);

                index++;
            }
        }

        public List<Vector3> GetPointList()
        {
            return subCloud;
        }
    }
}