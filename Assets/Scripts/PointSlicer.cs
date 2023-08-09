using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PointSlicer : MonoBehaviour
{
    [SerializeField] private Transform[] boundaryPoints;
    [SerializeField] private GameObject toBeSliced;
    public void SetSliceTarget(GameObject target){toBeSliced=target;}

    // Start is called before the first frame update
    void Start()
    {
        //Slice();
    }

    public void Slice(){
        for (int i = 0; i < boundaryPoints.Length; i++)
        {
            Vector3 pointA = new Vector3(); 
            Vector3 pointB = new Vector3();

            if(i+1 == boundaryPoints.Length && i>1){
                pointA = boundaryPoints[i].position;
                pointB = boundaryPoints[0].position;
                
            }else if(i+1 != boundaryPoints.Length){
                pointA = boundaryPoints[i].position;
                pointB = boundaryPoints[i+1].position;
            }
            Vector3 midPoint = Vector3.Lerp(pointA, pointB, 0.5f);
             
            Vector3 directionVector = pointA - pointB;
            Vector3 normalVector = new Vector3(-directionVector.z,0,directionVector.x);
            Vector3 directionFromSelfToMidpoint =  transform.position-midPoint;
            
            float dot = Vector3.Dot(directionFromSelfToMidpoint.normalized,normalVector.normalized);
            // Debug.Log(dot);
            // if(dot<1){
            //     //Debug.Log("unaligned");
            // }

            normalVector *=-1;

            // Get the direction from self to midpoint
            PlaneDrawer.Instance?.AddCenterNormal(midPoint,normalVector);

            SlicedHull hull = toBeSliced?.Slice(midPoint, normalVector, crossSectionMaterial);

            if (hull != null) {
                //Debug.Log("hull not null");
                GameObject lowerHull = hull.CreateLowerHull(toBeSliced, crossSectionMaterial);
                GameObject upperHull = hull.CreateUpperHull(toBeSliced, crossSectionMaterial);

                //TODO check for bugs again
                toBeSliced.SetActive(false);
                upperHull.SetActive(false);
                //Destroy(upperHull);
                //Destroy(sliceable);

                toBeSliced = lowerHull;
                MeshCollider mc = toBeSliced.AddComponent<MeshCollider>();
                mc.convex = true;
            }
            Debug.DrawLine(midPoint, transform.position, Color.red, 100f,false);
        }
        toBeSliced=null;
    }

    [SerializeField] private Material crossSectionMaterial;
    public void SetCrossSectionMaterial(Material crossMat){crossSectionMaterial=crossMat;}

    //Mostly here as reference
    public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial = null) {
        // slice the provided object using the transforms of this object
        return obj.Slice(transform.position, transform.up, crossSectionMaterial);
	}


    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    /// 
    [SerializeField]private float yLineGizmoHeight=5f;
    void OnDrawGizmos()
    {
        
        if(boundaryPoints.Length<2 || boundaryPoints.Length<1){
            return;
        }
        for (int i = 0; i < boundaryPoints.Length; i++)
        {
            if(boundaryPoints[i]==null)return;
            Gizmos.color = Color.magenta;
            if(i+1 == boundaryPoints.Length && i>1){
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(boundaryPoints[i].position, boundaryPoints[0].position);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(boundaryPoints[i].position
                    , boundaryPoints[i].position+new Vector3(0,yLineGizmoHeight,0));
                Gizmos.DrawLine(boundaryPoints[i].position+new Vector3(0,yLineGizmoHeight,0)
                    , boundaryPoints[0].position+new Vector3(0,yLineGizmoHeight,0));
            }else if(i+1 != boundaryPoints.Length){
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(boundaryPoints[i].position, boundaryPoints[i+1].position);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(boundaryPoints[i].position
                    , boundaryPoints[i].position+new Vector3(0,yLineGizmoHeight,0));
                Gizmos.DrawLine(boundaryPoints[i+1].position
                    , boundaryPoints[i+1].position+new Vector3(0,yLineGizmoHeight,0));
                Gizmos.DrawLine(boundaryPoints[i].position +new Vector3(0,yLineGizmoHeight,0)
                    , boundaryPoints[i+1].position+new Vector3(0,yLineGizmoHeight,0));
            }
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(PointSlicer))]
class PointSlicer1Editor : Editor{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PointSlicer pointSlicer = (PointSlicer) target;
        if (pointSlicer==null) return;

        if(GUILayout.Button("Test Slice")){
            pointSlicer.Slice();
        }
    }
}

#endif
