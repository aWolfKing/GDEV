using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataMeshTriangle {

    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;
    public int value;

    public DataMeshTriangle(){
        this.p0 = Vector3.zero;
        this.p1 = Vector3.zero;
        this.p2 = Vector3.zero;
        this.value = -2;
    }

    public DataMeshTriangle(Vector3 p0, Vector3 p1, Vector3 p2, int value){
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;
        this.value = value;
    }

}
