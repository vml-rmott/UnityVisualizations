using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class BezierCurveLineRenderer : MonoBehaviour
{

    public Transform[] points;

    private LineRenderer lineRenderer;
    public int vertexCount = 12;

    [Range(0f, 1f)]
    public float visiblePercentage = 1.0f;

	void Awake () {
        lineRenderer = GetComponent<LineRenderer>();
	}

    IEnumerator AnimatePoints()
    {
        while (true)
        {
            if (points == null || points.Length <= 0)
            {
                lineRenderer.positionCount = 0;
                lineRenderer.SetPositions(new Vector3[] { Vector3.zero });
            }
            else
            {
                var pointList = new List<Vector3>();
                for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
                {
                    Vector3 bezierPoint = CalculateBezierPoint(ratio, points.Select(point => point.position));
                    pointList.Add(bezierPoint);
                }
                lineRenderer.positionCount = (int)(pointList.Count * Mathf.Min(visiblePercentage, 0.9f));
                lineRenderer.SetPositions(pointList.ToArray());
            }

            yield return new WaitForEndOfFrame();
        }
    }
	
    private Vector3 CalculateBezierPoint(float ratio, IEnumerable<Vector3> points)
    {
        if (points.Count() == 1)
        {
            return points.First();
        }

        LinkedList<Vector3> subPoints = new LinkedList<Vector3>();
        Vector3? lastPoint = null;
        foreach (var point in points)
        {
            if (!lastPoint.HasValue)
            {
                lastPoint = point;
                continue;
            }
            else
            {
                subPoints.AddLast(Vector3.Lerp(lastPoint.Value, point, ratio));

                lastPoint = point;
            }
        }

        return CalculateBezierPoint(ratio, subPoints);
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(point1.position, point2.position);

        //Gizmos.color = Color.cyan;
        //Gizmos.DrawLine(point2.position, point3.position);

        //Gizmos.color = Color.red;
        //for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
        //{
        //    Gizmos.DrawLine(Vector3.Lerp(point1.position, point2.position, ratio),
        //        Vector3.Lerp(point2.position, point3.position, ratio));
        //}
    }

    void Start()
    {
        StartCoroutine(AnimatePoints());
    }
}
