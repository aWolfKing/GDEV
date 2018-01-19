using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalDataMesh {

    public List<DataMeshTriangle> triangles = new List<DataMeshTriangle>();

    public void Clear(){

    }


    public int GetValueBelow(Vector3 position) {
        return GetValue(position, -Vector3.up);
    }

    public int GetValue(Vector3 position, Vector3 direction) {
        Vector3 hit;
        DataMeshTriangle closest = DataMesh3DMath.GetFirstHitTriangle(position, direction, out hit, triangles.ToArray());
        if (closest != null) {
            return closest.value;
        }
        else {
            return -1;
        }
    }

    public DataMeshTriangle GetTriangle(Vector3 position, Vector3 direction) {
        Vector3 hit;
        return GetTriangle(position, direction, out hit);
    }

    public DataMeshTriangle GetTriangle(Vector3 position, Vector3 direction, out Vector3 hit) {
        return DataMesh3DMath.GetFirstHitTriangle(position, direction, out hit, triangles.ToArray());
    }


}
