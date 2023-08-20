using UnityEngine;
using System.Collections.Generic;

public class PlaneDrawer : MonoBehaviour
{
    public static PlaneDrawer Instance;

    public void Awake() {
        Instance = this;
    }

    public List<Vector3> normals, centerPoints;
    public Color color;
    public float size;

    public void AddCenterNormal(Vector3 cent, Vector3 norm)
    {
        centerPoints.Add(cent);
        normals.Add(norm);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < normals.Count; i++)
        {
            DrawPlane(normals[i], centerPoints[i], color, size);
        }
    }

    private void DrawPlane(Vector3 planeNormal, Vector3 planeCenter, Color planeColor, float planeSize)
    {
        Vector3 planeRight = Vector3.Cross(Vector3.up, planeNormal).normalized;
        Vector3 planeUp = Vector3.Cross(planeNormal, planeRight).normalized;

        Vector3 corner1 = planeCenter + planeRight * planeSize + planeUp * planeSize;
        Vector3 corner2 = planeCenter - planeRight * planeSize + planeUp * planeSize;
        Vector3 corner3 = planeCenter - planeRight * planeSize - planeUp * planeSize;
        Vector3 corner4 = planeCenter + planeRight * planeSize - planeUp * planeSize;

        Gizmos.color = planeColor;
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner4);
        Gizmos.DrawLine(corner4, corner1);
    }
}
