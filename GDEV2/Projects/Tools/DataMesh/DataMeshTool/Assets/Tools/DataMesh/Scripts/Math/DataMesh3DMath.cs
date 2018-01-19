using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://wiki.unity3d.com/index.php/3d_Math_functions

public class DataMesh3DMath {


    public static Vector3 SetVectorLength(Vector3 vector, float size) {
        Vector3 vectorNormalized = Vector3.Normalize(vector);
        return vectorNormalized *= size;
    }
    
    public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint) {

        float length;
        float dotNumerator;
        float dotDenominator;
        Vector3 vector;
        intersection = Vector3.zero;

        //calculate the distance between the linePoint and the line-plane intersection point
        dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3.Dot(lineVec, planeNormal);

        //line and plane are not parallel
        if (dotDenominator != 0.0f) {
            length = dotNumerator / dotDenominator;

            //create a vector from the linePoint to the intersection point
            vector = SetVectorLength(lineVec, length);

            //get the coordinates of the line-plane intersection point
            intersection = linePoint + vector;

            return true;
        }

        //output not valid
        else {
            return false;
        }
    }



    public static bool IsInTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 pt, Vector3 dir, float maxDist, out Vector3 position) {
        return IsInTriangle(p0, p1, p2, pt, dir, out position) && Vector3.Distance(position, pt) <= maxDist;
    }

    public static bool IsInTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 pt, Vector3 dir) {
        Vector3 vec;
        return IsInTriangle(p0, p1, p2, pt, dir, out vec);
    }

    public static bool IsInTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 pt, Vector3 dir, out Vector3 position) {
        
        if(p0.y == p1.y && p1.y == p2.y){
            p0 += Vector3.up * 0.001f;
        }
        if(p0.x == p1.x && p1.x == p2.x){
            p0 += Vector3.right * 0.001f;
        }
        if(p0.z == p1.z && p1.z == p2.z){
            p0 += Vector3.forward * 0.001f;
        }
        
        if (LinePlaneIntersection(out position, pt, dir, Vector3.Cross((p1 - p0).normalized, (p2 - p0).normalized), (p0 + p1 + p2) / 3)) {

            float X1 = p0.x, X2 = p1.x, X3 = p2.x, X4 = position.x, Y1 = p0.y, Y2 = p1.y, Y3 = p2.y, Y4 = position.y;
            float detA = ((X1 * Y2) - (X1 * Y3) - (X2 * Y1) + (X2 * Y3) + (X3 * Y1) - (X3 * Y2));
            float u = ((X4 * Y2) - (X4 * Y3) - (X2 * Y4) + (X2 * Y3) + (X3 * Y4) - (X3 * Y2)) / detA;
            float v = ((X1 * Y4) - (X1 * Y3) - (X4 * Y1) + (X4 * Y3) + (X3 * Y1) - (X3 * Y4)) / detA;
            float w = ((X1 * Y2) - (X1 * Y4) - (X2 * Y1) + (X2 * Y4) + (X4 * Y1) - (X4 * Y2)) / detA;

            //UnityEditor.Handles.Label((p0 + p1 + p2) / 3, u + ", " + v + ", " + w);

            return u >= 0 && u <= 1 && v >= 0 && v <= 1 && w >= 0 && w <= 1;
        }
        return false;

    }



    public static DataMeshTriangle GetFirstHitTriangle(Vector3 pos, Vector3 dir, out Vector3 hit, params DataMeshTriangle[] tris){
        Vector3? closestHit = null;
        DataMeshTriangle closest = null;
        for(int i=0; i<tris.Length; i++){
            Vector3 h;
            if(IsInTriangle(tris[i].p0, tris[i].p1, tris[i].p2, pos, dir, out h)){
                if (closestHit.HasValue){
                    if(Vector3.Distance(pos, closestHit.Value) > Vector3.Distance(pos, h)){
                        closest = tris[i];
                        closestHit = h;
                    }
                }
                else {
                    closest = tris[i];
                    closestHit = h;
                }
            }
        }
        if(closestHit.HasValue){ hit = closestHit.Value; }
        else{ hit = Vector3.zero; }
        return closest;
    }

    public static DataMeshTriangle GetFirstHitTriangle(Vector3 pos, Vector3 dir, params DataMeshTriangle[] tris){
        Vector3 hit;
        return GetFirstHitTriangle(pos, dir, out hit, tris);
    }
}
